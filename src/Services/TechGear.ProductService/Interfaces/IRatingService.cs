using TechGear.ProductService.DTOs;
using TechGear.ProductService.Models;

namespace TechGear.ProductService.Interfaces
{
    public interface IRatingService
    {
        Task<IEnumerable<RatingReviewDto>> GetRatingsByProductIdAsync(int productItemId);
        Task<IEnumerable<RatingReviewDto>> GetRatingsByUserIdAsync(int userId);
        Task<AverageRatingDto> GetAverageRatingForProductIdAsync(int productId);
        Task<bool> IsRated(int orderId, int productItemId);
        Task<bool> AddRatingAsync(RatingDto rating);
        Task<bool> UpdateRatingAsync(RatingDto rating);
        Task<bool> DeleteRatingAsync(int ratingId);
    }
}
