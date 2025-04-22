using TechGear.OrderService.DTOs;

namespace TechGear.OrderService.Interfaces
{
    public interface IEmailService
    {
        Task SendOrderConfirmationEmailAsync(OrderEmailDto orderId);
    }
}
