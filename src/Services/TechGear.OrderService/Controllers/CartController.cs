using Microsoft.AspNetCore.Mvc;
using TechGear.OrderService.DTOs;
using TechGear.OrderService.Interfaces;

namespace TechGear.OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController(ICartService cartService) : ControllerBase
    {
       private readonly ICartService _cartService = cartService;

       [HttpGet("by-userId/{userId}")]
       public async Task<IActionResult> GetCartItemsByUserId(int userId)
       {
           var cartItems = await _cartService.GetAllCartItemsByUserId(userId);

           return Ok(cartItems);
       }

       [HttpPost("add")]
       public async Task<IActionResult> AddToCart([FromBody] ActionCartItemDto cartItem)
       {
           var added = await _cartService.AddToCartAsync(cartItem.UserId, cartItem.ProductItemId);

           if (!added)
           {
               return BadRequest(new { message = "This product item already exists in this user's cart" });
           }

           return Ok(new { message = "Item added to cart successfully." });
       }

       [HttpDelete("delete")]
       public async Task<IActionResult> DeleteCartItem([FromBody] ActionCartItemDto cartItem)
       {
           var deleted = await _cartService.DeleteAsync(cartItem.UserId, cartItem.ProductItemId);

           if (!deleted)
           {
               return NotFound(new { message = "This product item in user cart not found" });
           }

           return Ok(new { message = "Item deleted to cart successfully." });
        }
    }
}
