using Microsoft.AspNetCore.Mvc;
using TechGear.UserService.DTOs;
using TechGear.UserService.Interfaces;

namespace TechGear.UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteController(IFavoriteService favoriteService) : Controller
    {
        private readonly IFavoriteService _favoriteService = favoriteService;

        [HttpPost("add")]
        public async Task<IActionResult> AddToFavorites([FromBody] FavoriteDto dto)
        {
            var result = await _favoriteService.AddToFavoritesAsync(dto.UserId, dto.ProductId);
            if (result)
            {
                return Ok(new { message = "Product added to favorites." });
            }

            return BadRequest(new { message = "Product already in favorites." });
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> RemoveFromFavorites([FromBody] FavoriteDto dto)
        {
            var result = await _favoriteService.RemoveFromFavoritesAsync(dto.UserId, dto.ProductId);
            if (result)
            {
                return Ok(new { message = "Product removed from favorites." });
            }

            return BadRequest(new { message = "Product not in favorites." });
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetFavoriteProducts(int userId)
        {
            var result = await _favoriteService.GetFavoriteProductsAsync(userId);
            if (result.Count > 0)
            {
                return Ok(result);
            }

            return NotFound(new { message = "No favorite products found." });
        }

        [HttpPut("is-favorite")]
        public async Task<IActionResult> IsProductFavorite([FromBody] FavoriteDto dto)
        {
            var result = await _favoriteService.IsProductFavoriteAsync(dto.UserId, dto.ProductId);
            if (result)
            {
                return Ok(new { message = "Product is in favorites." });
            }

            return NotFound(new { message = "Product not in favorites." });
        }

    }
}
