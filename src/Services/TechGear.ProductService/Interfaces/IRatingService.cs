using TechGear.ProductService.DTOs;
using TechGear.ProductService.Models;

namespace TechGear.ProductService.Interfaces
{
    public interface IRatingService
    {
        Task<IEnumerable<Rating>> GetRatingsByProductIdAsync(int productId);
        Task<AverageRatingDto> GetAverageRatingForProductIdAsync(int productId);
        Task<bool> AddRatingAsync(Rating rating);
        Task<bool> UpdateRatingAsync(Rating rating);
        Task<bool> DeleteRatingAsync(int ratingId);

    }
}
