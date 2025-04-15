using Microsoft.AspNetCore.Mvc;
using TechGear.ProductService.Interfaces;

namespace TechGear.ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController(IRatingService ratingService) : ControllerBase
    {
        private readonly IRatingService _ratingService = ratingService;

        [HttpGet("average/{productId}")]
        public async Task<IActionResult> GetAverageRatingForProductId(int productId)
        {
            var result = await _ratingService.GetAverageRatingForProductIdAsync(productId);

            return Ok(result);
        }
    }
}
