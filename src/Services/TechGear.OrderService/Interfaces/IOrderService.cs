using TechGear.OrderService.DTOs;

namespace TechGear.OrderService.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<OrderDto?> GetOrderByIdAsync(int orderId);
        Task<OrderEmailDto?> CreateOrderAsync(OrderDto order);
        Task<bool> UpdateOrderAsync(OrderDto order);
        Task<bool> DeleteOrderAsync(int orderId);
    }
}
