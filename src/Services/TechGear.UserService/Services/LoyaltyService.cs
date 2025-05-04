using Microsoft.EntityFrameworkCore;
using TechGear.UserService.Data;
using TechGear.UserService.Interfaces;
using TechGear.UserService.Models;

namespace TechGear.UserService.Services
{
    public class LoyaltyService(TechGearUserServiceContext context) : ILoyaltyService
    {
        private readonly TechGearUserServiceContext _context = context;

        public async Task<IEnumerable<Loyalty>> GetAllLoyaltiesByUserIdAsync(int userId)
        {
            return await _context.Loyalties
                .Where(l => l.UserId == userId)
                .ToListAsync();
        }

        public async Task<Loyalty?> GetLoyaltyByIdAsync(int id)
        {
            return await _context.Loyalties
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<bool> AddLoyaltyPointsAsync(int userId, int fromOrderId, int point, string action)
        {
            var existLoyalty = await _context.Loyalties
                .FirstOrDefaultAsync(l => l.UserId == userId && l.FromOrderId == fromOrderId);
            if (existLoyalty != null)
            {
                return false;
            }

            var newLoyalty = new Models.Loyalty
            {
                UserId = userId,
                FromOrderId = fromOrderId,
                Point = point,
                Action = action,
                CreatedAt = DateTime.UtcNow.AddHours(7)
            };

            _context.Loyalties.Add(newLoyalty);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
