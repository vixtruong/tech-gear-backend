using Microsoft.EntityFrameworkCore;
using TechGear.ProductService.Data;
using TechGear.ProductService.DTOs;
using TechGear.ProductService.Interfaces;
using TechGear.ProductService.Models;

namespace TechGear.ProductService.Services
{
    public class RatingService(TechGearProductServiceContext context) : IRatingService
    {
        private readonly TechGearProductServiceContext _context = context;


        public async Task<IEnumerable<Rating>> GetRatingsByProductIdAsync(int productId)
        {
            return await _context.Ratings.Where(r => r.ProductId == productId).ToListAsync();
        }

        public async Task<AverageRatingDto> GetAverageRatingForProductIdAsync(int productId)
        {
            var ratings = await _context.Ratings
                .Where(r => r.ProductId == productId)
                .ToListAsync();

            if (ratings.Count == 0)
            {
                return new AverageRatingDto
                {
                    ProductId = productId,
                    RatingCount = 0,
                    AverageRating = 0
                };
            }

            return new AverageRatingDto
            {
                ProductId = productId,
                AverageRating = ratings.Average(r => r.Star),
                RatingCount = ratings.Count
            };
        }

        public async Task<bool> AddRatingAsync(Rating rating)
        {
            var existRating =
                await _context.Ratings.FirstOrDefaultAsync(r =>
                    r.UserId == rating.UserId && r.ProductId == rating.ProductId);

            if (existRating != null) return false;

            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateRatingAsync(Rating rating)
        {
            var existRating = await _context.Ratings.FindAsync(rating.Id);

            if (existRating == null) return false;

            existRating.Star = rating.Star;
            existRating.Content = rating.Content;
            existRating.LastUpdate = DateTime.Now;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteRatingAsync(int ratingId)
        {
            var existRating = await _context.Ratings.FindAsync(ratingId);

            if (existRating == null) return false;

            _context.Ratings.Remove(existRating);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
