using Microsoft.EntityFrameworkCore;
using TechGear.UserService.Data;
using TechGear.UserService.Interfaces;
using TechGear.UserService.Models;

namespace TechGear.UserService.Services
{
    public class FavoriteService(TechGearUserServiceContext _context) : IFavoriteService
    {
        public async Task<bool> AddToFavoritesAsync(int userId, int productId)
        {
            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);

            if (favorite != null)
            {
                return false; // Already in favorites
            }

            favorite = new Favorite
            {
                UserId = userId,
                ProductId = productId,
            };

            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();

            return true; // Successfully added to favorites
        }

        public async Task<bool> RemoveFromFavoritesAsync(int userId, int productId)
        {
            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);

            if (favorite == null)
            {
                return false; // Not in favorites
            }

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();

            return true; // Successfully removed from favorites
        }

        public async Task<List<int>> GetFavoriteProductsAsync(int userId)
        {
            return await _context.Favorites
                .Where(f => f.UserId == userId)
                .Select(f => f.ProductId)
                .ToListAsync();
        }

        public async Task<bool> IsProductFavoriteAsync(int userId, int productId)
        {
            return await _context.Favorites
                .AnyAsync(f => f.UserId == userId && f.ProductId == productId);
        }
    }
}
