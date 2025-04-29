using Microsoft.AspNetCore.Mvc;
using TechGear.ProductService.DTOs;
using TechGear.ProductService.Interfaces;
using TechGear.ProductService.Models;

namespace TechGear.ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController(IRatingService ratingService) : ControllerBase
    {
        private readonly IRatingService _ratingService = ratingService;

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetRatingsByProductItemId(int productId)
        {
            var result = await _ratingService.GetRatingsByProductIdAsync(productId);

            return Ok(result);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetRatingsByUserId(int userId)
        {
            var result = await _ratingService.GetRatingsByUserIdAsync(userId);

            return Ok(result);
        }

        [HttpGet("average/{productId}")]
        public async Task<IActionResult> GetAverageRatingForProductId(int productId)
        {
            var result = await _ratingService.GetAverageRatingForProductIdAsync(productId);

            return Ok(result);
        }

        [HttpGet("is-rated/{orderId}/{productItemId}")]
        public async Task<IActionResult> IsRated(int orderId, int productItemId)
        {
            var result = await _ratingService.IsRated(orderId, productItemId);

            return Ok(result);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddRating([FromBody] RatingDto? rating)
        {
            Console.WriteLine($"Rating: {rating.UserId} - {rating.OrderId} - {rating.Star} - {rating.Content} - {rating.ProductItemId} - {rating.LastUpdate}");
            if (rating == null)
            {
                return BadRequest("Invalid rating data.");
            }

            var result = await _ratingService.AddRatingAsync(rating);

            if (result)
            {
                return CreatedAtAction(nameof(GetRatingsByProductItemId), new { productItemId = rating.ProductItemId }, rating);
            }

            return BadRequest("Failed to add rating.");
        }

        [HttpPut("{ratingId}")]
        public async Task<IActionResult> UpdateRating(int ratingId, [FromBody] RatingDto? rating)
        {
            if (rating == null || rating.Id != ratingId)
            {
                return BadRequest("Invalid rating data.");
            }

            var result = await _ratingService.UpdateRatingAsync(rating);

            if (result)
            {
                return NoContent();
            }

            return NotFound("Rating not found.");
        }

        [HttpDelete("{ratingId}")]
        public async Task<IActionResult> DeleteRating(int ratingId)
        {
            var result = await _ratingService.DeleteRatingAsync(ratingId);

            if (result)
            {
                return NoContent();
            }

            return NotFound("Rating not found.");
        }
    }
}
