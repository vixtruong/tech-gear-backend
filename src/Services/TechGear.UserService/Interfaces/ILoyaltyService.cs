using TechGear.UserService.Models;

namespace TechGear.UserService.Interfaces
{
    public interface ILoyaltyService
    {
        Task<IEnumerable<Loyalty>> GetAllLoyaltiesByUserIdAsync(int userId);
        Task<Loyalty?> GetLoyaltyByIdAsync(int id);
        Task<bool> AddLoyaltyPointsAsync(int userId, int fromOrderId, int points);
    }
}
