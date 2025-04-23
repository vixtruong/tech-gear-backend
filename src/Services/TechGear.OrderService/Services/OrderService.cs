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

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ToListAsync();

            return orders.Select(o => new OrderDto
            {
                Id = o.Id,
                UserId = o.UserId,
                UserAddressId = o.UserAddressId,
                CouponId = o.CouponId,
                TotalAmount = o.TotalAmount,
                CreatedAt = o.CreateAt,
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
                    couponValue = await _context.Coupons
                        .Where(c => c.Id == order.CouponId)
                        .Select(c => c.Value)
                        .FirstOrDefaultAsync();

                    paymentAmount -= couponValue;
                }

                var client = _httpClientFactory.CreateClient("ApiGatewayClient");
                var userResponse = await client.GetFromJsonAsync<UserInfoResponse>(
                    $"api/v1/users/{order.UserId}?userAddressId={order.UserAddressId}");

                if (order.IsUsePoint == true)
                {
                    var maxPointToUse = Math.Min(userResponse?.Point ?? 0, paymentAmount / 1000);
                    paymentAmount -= maxPointToUse * 1000;

                    newOrder.Point = maxPointToUse;

                    var response = await client.PutAsJsonAsync(
                        $"api/v1/users/{order.UserId}/points", maxPointToUse);

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
    }
}
