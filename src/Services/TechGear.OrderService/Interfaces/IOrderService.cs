using TechGear.OrderService.DTOs;

namespace TechGear.OrderService.Interfaces
{
    public interface IOrderService
    {
        Task<TotalOrderDto> GetTotalOrderAsync();
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(int userId);
        Task<OrderDto?> GetOrderByIdAsync(int orderId);
        Task<OrderDetailDto?> GetOrderDetailByIdAsync(int orderId);
        Task<IEnumerable<ProductItemInfoDto>?> GetOrderItemsInfoByOrderId(int orderId);
        Task<OrderEmailDto?> CreateOrderAsync(OrderDto order);
        Task<bool> UpdateOrderAsync(OrderDto order);
        Task<bool> UpdateOrderStatusAsync(OrderStatusDto dto);
        Task<bool> DeleteOrderAsync(int orderId);
        Task<bool> IsValidRating(int orderId);
    }
}
