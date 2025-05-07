using Microsoft.EntityFrameworkCore;
using TechGear.OrderService.Data;
using TechGear.OrderService.DTOs;
using TechGear.OrderService.Interfaces;
using TechGear.OrderService.Models;

namespace TechGear.OrderService.Services
{
    public class OrderService(TechGearOrderServiceContext context, IHttpClientFactory httpClientFactory) : IOrderService
    {
        private readonly TechGearOrderServiceContext _context = context;
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

        public async Task<TotalOrderDto> GetTotalOrderAsync()
        {
            var totalOrders = await _context.Orders.CountAsync();
            var totalRevenue = await _context.Payments.Where(o => o.PaidAt != null).SumAsync(o => o.Amount);

            return new TotalOrderDto
            {
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue
            };
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.Payments)
                .Include(o => o.Delivery)
                .ToListAsync();

            return orders.Select(o => new OrderDto
            {
                Id = o.Id,
                UserId = o.UserId,
                UserAddressId = o.UserAddressId,
                CouponId = o.CouponId,
                TotalAmount = o.TotalAmount,
                PaymentAmount = o.Payments.FirstOrDefault()?.Amount,
                PaymentMethod = o.Payments.FirstOrDefault()?.Method!,
                CreatedAt = o.CreateAt,
                DeliveredDate = o.Delivery.DeliveryDate,
                ConfirmedDate = o.Delivery.ConfirmDate,
                ShippedDate = o.Delivery.ShipDate,
                CanceledDate = o.Delivery.CancelDate,
                CancelReason = o.Delivery.CancelReason,
                Status = o.Delivery.Status,
                OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    ProductItemId = oi.ProductItemId,
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList()
            });
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(int userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .Include(o => o.Payments)
                .Include(o => o.Delivery)
                .ToListAsync();

            return orders.Select(o => new OrderDto
            {
                Id = o.Id,
                UserId = o.UserId,
                UserAddressId = o.UserAddressId,
                CouponId = o.CouponId,
                TotalAmount = o.TotalAmount,
                PaymentAmount = o.Payments.FirstOrDefault()?.Amount,
                PaymentMethod = o.Payments.FirstOrDefault()?.Method!,
                CreatedAt = o.CreateAt,
                DeliveredDate = o.Delivery.DeliveryDate,
                ConfirmedDate = o.Delivery.ConfirmDate,
                ShippedDate = o.Delivery.ShipDate,
                CanceledDate = o.Delivery.CancelDate,
                CancelReason = o.Delivery.CancelReason,
                Status = o.Delivery.Status,
                OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    ProductItemId = oi.ProductItemId,
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList()
            });
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return null;
            }

            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                UserAddressId = order.UserAddressId,
                CouponId = order.CouponId,
                TotalAmount = order.TotalAmount,
                CreatedAt = order.CreateAt,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    ProductItemId = oi.ProductItemId,
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList()
            };
        }

        public async Task<OrderDetailDto?> GetOrderDetailByIdAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.Payments)
                .Include(o => o.Delivery)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                Console.WriteLine($"Không tìm thấy đơn hàng với ID {orderId}");
                return null;
            }

            var productItemIds = order.OrderItems.Select(oi => oi.ProductItemId).ToList();

            var client = _httpClientFactory.CreateClient("ApiGatewayClient");

            var userResponse = await client.GetFromJsonAsync<UserAddressInfoDto>(
                $"api/v1/users/{order.UserId}/address?userAddressId={order.UserAddressId}");

            if (userResponse == null)
            {
                Console.WriteLine("Không lấy được thông tin user.");
                throw new Exception("User info not found.");
            }

            var productItemResponse = await client.PostAsJsonAsync("api/v1/productItems/by-ids", productItemIds);
            if (!productItemResponse.IsSuccessStatusCode)
                throw new Exception("Không thể lấy thông tin sản phẩm.");

            var productItems = await productItemResponse.Content.ReadFromJsonAsync<List<ProductItemInfoDto>>();

            if (productItems == null)
                throw new Exception("Dữ liệu sản phẩm rỗng.");

            var orderItems = order.OrderItems.Select(orderItem =>
            { 
                var productInfo = productItems.FirstOrDefault(p => p.ProductItemId == orderItem.ProductItemId);

                return new OrderItemDetailDto
                {
                    ProductItemId = orderItem.ProductItemId,
                    ProductName = productInfo?.ProductName ?? "",
                    Sku = productInfo?.Sku ?? "",
                    ImageUrl = productInfo?.ImageUrl ?? "",
                    Price = productInfo?.Price ?? 0,
                    Discount = productInfo?.Discount ?? 0,
                    Quantity = orderItem.Quantity,
                    TotalPrice = orderItem.Quantity * (int)(productInfo?.Price ?? 0 * (1 - (productInfo?.Discount ?? 0) / 100.0m))
                };
            }).ToList();


            var coupon = await _context.Coupons
                .FirstOrDefaultAsync(c => c.Id == order.CouponId);

            var firstPayment = order.Payments.FirstOrDefault();

            var orderDetailDto = new OrderDetailDto
            {
                OrderId = order.Id,
                UserEmail = userResponse.Email,
                RecipientName = userResponse.RecipientName ?? "",
                RecipientPhone = userResponse.RecipientPhone ?? "",
                Address = userResponse.Address ?? "",
                CouponCode = coupon?.Code ?? "",
                Point = order.Point,
                OrderTotalPrice = order.TotalAmount,
                PaymentTotalPrice = firstPayment?.Amount ?? 0,
                PaymentMethod = firstPayment?.Method ?? "Unknown",
                PaymentStatus = firstPayment?.PaidAt == null ? "Unpaid" : "Paid",
                CreatedAt = order.CreateAt,
                OrderItems = orderItems,
            };

            return orderDetailDto;
        }


        public async Task<IEnumerable<ProductItemInfoDto>?> GetOrderItemsInfoByOrderId(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null || order?.OrderItems == null || order.OrderItems.Count == 0)
            {
                return null;
            }

            var productItemIds = order.OrderItems.Select(oi => oi.ProductItemId).ToList();

            var client = _httpClientFactory.CreateClient("ApiGatewayClient");

            var productItemResponse = await client.PostAsJsonAsync("api/v1/productItems/by-ids", productItemIds);
            if (!productItemResponse.IsSuccessStatusCode)
                throw new Exception("Can not get information.");

            var productItems = await productItemResponse.Content.ReadFromJsonAsync<List<ProductItemInfoDto>>();

            if (productItems == null)
                throw new Exception("Can not get information.");

            return productItems;
        }

        public async Task<OrderEmailDto?> CreateOrderAsync(OrderDto order)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Tạo Order
                var newOrder = new Order
                {
                    UserId = order.UserId,
                    UserAddressId = order.UserAddressId,
                    CouponId = order.CouponId,
                    TotalAmount = order.TotalAmount,
                    CreateAt = DateTime.UtcNow.AddHours(7),
                    OrderItems = order.OrderItems!.Select(oi => new OrderItem
                    {
                        ProductItemId = oi.ProductItemId,
                        Quantity = oi.Quantity,
                        Price = oi.Price,
                    }).ToList()
                };

                await _context.Orders.AddAsync(newOrder);
                await _context.SaveChangesAsync();

                var paymentAmount = order.TotalAmount;
                var couponValue = 0;

                if (order.CouponId != null)
                {
                    var coupon = await _context.Coupons.FindAsync(order.CouponId);

                    if (coupon != null && coupon.UsageLimit > 0)
                    {
                        couponValue = coupon.Value;
                        paymentAmount -= couponValue;
                        coupon.UsageLimit--;

                        await _context.SaveChangesAsync();
                    }
                }

                var client = _httpClientFactory.CreateClient("ApiGatewayClient");
                var userResponse = await client.GetFromJsonAsync<UserInfoResponse>(
                    $"api/v1/users/{order.UserId}?userAddressId={order.UserAddressId}");

                if (order.IsUsePoint == true)
                {
                    var maxPointToUse = Math.Min(userResponse?.Point ?? 0, paymentAmount / 1000);
                    paymentAmount -= maxPointToUse * 1000;

                    newOrder.Point = maxPointToUse;

                    var usePointDto = new UsePointDto
                    {
                        Point = maxPointToUse,
                        Action = "use",
                        OrderId = newOrder.Id
                    };

                    var response = await client.PutAsJsonAsync(
                        $"api/v1/users/{order.UserId}/points", usePointDto);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception("Failed to update user points.");
                    }
                }

                // Tạo Payment sau khi có OrderId
                var newPayment = new Payment
                {
                    OrderId = newOrder.Id,
                    Amount = paymentAmount,
                    Method = order.PaymentMethod,
                };
                await _context.Payments.AddAsync(newPayment);

                // Tạo Delivery sau khi có OrderId
                var newShipping = new Delivery
                {
                    OrderId = newOrder.Id,
                    Note = order.Note,
                    Status = "Pending",
                };
                await _context.Deliveries.AddAsync(newShipping);

                var productItemIds = order.OrderItems!.Select(oi => oi.ProductItemId).ToList();
                var productItemResponse = await client.PostAsJsonAsync("api/v1/productItems/by-ids", productItemIds);
                if (!productItemResponse.IsSuccessStatusCode)
                    throw new Exception("Can not get information.");

                var productItems = await productItemResponse.Content.ReadFromJsonAsync<List<ProductItemInfoDto>>();

                if (productItems == null)
                    throw new Exception("Can not get information.");

                var orderItemEmailDtos = newOrder.OrderItems.Select(orderItem =>
                {
                    var productInfo = productItems.FirstOrDefault(p => p.ProductItemId == orderItem.ProductItemId);

                    return new OrderItemEmailDto
                    {
                        ProductName = productInfo?.ProductName ?? "",
                        Sku = productInfo?.Sku ?? "",
                        ImageUrl = productInfo?.ImageUrl ?? "",
                        Quantity = orderItem.Quantity,
                        UnitPrice = orderItem.Price,
                        TotalPrice = orderItem.Quantity * orderItem.Price
                    };
                }).ToList();

                var orderEmailDto = new OrderEmailDto
                {
                    CustomerName = userResponse?.FullName ?? "",
                    Email = userResponse?.Email ?? "",
                    PhoneNumber = userResponse?.PhoneNumber ?? "",
                    Address = userResponse?.Address ?? "",
                    OrderId = newOrder.Id,
                    OrderDate = newOrder.CreateAt,
                    OriginalAmount = order.TotalAmount,
                    FinalAmount = paymentAmount,
                    PaymentMethod = order.PaymentMethod,
                    Note = order.Note,
                    Items = orderItemEmailDtos,
                    UsedPoints = newOrder.Point,
                    DiscountValue = couponValue
                };

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return orderEmailDto;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateOrderAsync(OrderDto order)
        {
            var existingOrder = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == order.Id);

            if (existingOrder == null)
            {
                return false;
            }

            existingOrder.UserId = order.UserId;
            existingOrder.UserAddressId = order.UserAddressId;
            existingOrder.CouponId = order.CouponId;
            existingOrder.TotalAmount = order.TotalAmount;

            // Update OrderItems
            foreach (var item in order.OrderItems!)
            {
                var existingItem = existingOrder.OrderItems.FirstOrDefault(oi => oi.Id == item.Id);
                if (existingItem != null)
                {
                    existingItem.Quantity = item.Quantity;
                    existingItem.Price = item.Price;
                }
                else
                {
                    existingOrder.OrderItems.Add(new OrderItem
                    {
                        ProductItemId = item.ProductItemId,
                        Quantity = item.Quantity,
                        Price = item.Price,
                    });
                }
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateOrderStatusAsync(OrderStatusDto dto)
        {
            var delivery = await _context.Deliveries
                .FirstOrDefaultAsync(d => d.OrderId == dto.OrderId);

            if (delivery == null)
            {
                return false;
            }

            delivery.Status = dto.Status;
            if (dto.Status == "Delivered")
            {
                delivery.DeliveryDate = DateTime.UtcNow.AddHours(7);

                var payment = _context.Payments.FirstOrDefault(p => p.OrderId == dto.OrderId);
                if (payment != null)
                {
                    payment.PaidAt = DateTime.UtcNow.AddHours(7);
                    _context.Payments.Update(payment);
                }
            }
            else if (dto.Status == "Canceled")
            {
                delivery.CancelDate = DateTime.UtcNow.AddHours(7);
                delivery.CancelReason = dto.CancelReason;

                var payment = _context.Payments.FirstOrDefault(p => p.OrderId == dto.OrderId);

                if (payment != null)
                {
                    payment.PaidAt = null;
                    _context.Payments.Update(payment);
                }
            }
            else if (dto.Status == "Confirmed")
            {
                delivery.ConfirmDate = DateTime.UtcNow.AddHours(7);
            }
            else if (dto.Status == "Shipped")
            {
                delivery.ShipDate = DateTime.UtcNow.AddHours(7);
            }

            _context.Deliveries.Update(delivery);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return false;
            }

            _context.Orders.Remove(order);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> IsValidRating(int orderId)
        {
            var deliveryDate = await _context.Deliveries
                .Where(d => d.OrderId == orderId)
                .Select(d => d.DeliveryDate)
                .FirstOrDefaultAsync();

            if (deliveryDate == null)
            {
                return false;
            }

            var daysSinceDelivery = (DateTime.Now - deliveryDate.Value).TotalDays;

            if (daysSinceDelivery <= 14)
            {
                return true;
            }

            return false;
        }

    }
}
