using TechGear.OrderService.DTOs;
using TechGear.OrderService.Models;

namespace TechGear.OrderService.Interfaces
{
    public interface ICartService
    {
        Task<IEnumerable<CartItemDto>?> GetAllCartItemsByUserId(int userId);
        Task<bool> AddToCartAsync(int userId,int productId );
        Task<bool> DeleteAsync(int userId, int productId);
        Task<bool> UpdateAsync(int userId, int productId, int quantity);
    }
}
