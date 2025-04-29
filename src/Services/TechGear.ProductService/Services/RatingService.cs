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

        public async Task<IEnumerable<RatingReviewDto>> GetRatingsByProductIdAsync(int producId)
        {
            var result = await _context.Ratings
                .Include(r => r.ProductItem)
                .ThenInclude(pi => pi.Product)
                .Where(r => r.ProductItem.ProductId == producId)
                .ToListAsync();

            return result.Select(r => new RatingReviewDto
            {
                Id = r.Id,
                OrderId = r.OrderId,
                ProductItemId = r.ProductItemId,
                UserId = r.UserId,
                ImgUrl = r.ProductItem.ProductImage,
                ProductName = r.ProductItem.Product.Name,
                Sku = r.ProductItem.Sku,
                Star = r.Star,
                Content = r.Content,
                LastUpdate = r.LastUpdate
            });
        }

        public async Task<IEnumerable<RatingReviewDto>> GetRatingsByUserIdAsync(int userId)
        {
            var result =  await _context.Ratings.Where(r => r.UserId == userId)
                .Include(r => r.ProductItem)
                .ThenInclude(pi => pi.Product)
                .ToListAsync();

            return result.Select(r => new RatingReviewDto
            {
                Id = r.Id,
                OrderId = r.OrderId,
                ProductItemId = r.ProductItemId,
                UserId = r.UserId,
                ImgUrl = r.ProductItem.ProductImage,
                ProductName = r.ProductItem.Product.Name,
                Sku = r.ProductItem.Sku,
                Star = r.Star,
                Content = r.Content,
                LastUpdate = r.LastUpdate
            });
        }

        public async Task<AverageRatingDto> GetAverageRatingForProductIdAsync(int productId)
        {
            var productItems = await _context.ProductItems
                .Include(p => p.Ratings)
                .Where(p => p.ProductId == productId)
                .ToListAsync();

            var ratings = productItems.SelectMany(p => p.Ratings).ToList();

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

        public async Task<bool> IsRated(int orderId, int productItemId)
        {
            return await _context.Ratings.AnyAsync(r => r.OrderId == orderId && r.ProductItemId == productItemId);
        }

        public async Task<bool> AddRatingAsync(RatingDto rating)
        {
            var existRating = await _context.Ratings.FirstOrDefaultAsync(r =>
                r.UserId == rating.UserId &&
                r.ProductItemId == rating.ProductItemId &&
                r.OrderId == rating.OrderId);

            if (existRating != null) return false;

            var entity = new Rating
            {
                ProductItemId = rating.ProductItemId,
                OrderId = rating.OrderId,
                UserId = rating.UserId,
                Star = rating.Star,
                Content = rating.Content,
                LastUpdate = rating.LastUpdate
            };

            _context.Ratings.Add(entity);
            await _context.SaveChangesAsync();

            return true;
        }


        public async Task<bool> UpdateRatingAsync(RatingDto rating)
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
