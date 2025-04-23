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

       [HttpGet("{userId}")]
       public async Task<IActionResult> GetCartItemsByUserId(int userId)
       {
           var cartItems = await _cartService.GetAllCartItemsByUserId(userId);

           return Ok(cartItems);
       }


       [HttpPost("add")]
       public async Task<IActionResult> AddToCart([FromBody] ActionCartItemDto cartItem)
       {
           Console.WriteLine("quantity" + cartItem.Quantity);
           var added = await _cartService.AddToCartAsync(cartItem.UserId, cartItem.ProductItemId, cartItem.Quantity);

           if (!added)
           {
               return BadRequest(new { message = "This product item already exists in this user's cart" });
           }

           return Ok(new { message = "Item added to cart successfully." });
       }

       [HttpPost("update")]
       public async Task<IActionResult> UpdateListToCart([FromBody] CartListDto cartListItem)
       {
           var added = await _cartService.UpdateListToCartAsync(cartListItem.UserId, cartListItem.CartItems!);

           if (!added)
           {
               return BadRequest(new { message = "Failed." });
           }

           return Ok(new { message = "Items added to cart successfully." });
       }

        [HttpPut("update-quantity")]
        public async Task<IActionResult> UpdateCartItem([FromBody] ActionCartItemDto cartItem)
        {
            var updated = await _cartService.UpdateAsync(cartItem.UserId, cartItem.ProductItemId, cartItem.Quantity ?? 1);

            if (!updated)
            {
                return NotFound(new { message = "This product item in user cart not found" });
            }

            return Ok(new { message = "Item updated to cart successfully." });
        }

       [HttpDelete("delete/{productItemId}")]
       public async Task<IActionResult> DeleteCartItem(int productItemId,[FromBody] ActionCartItemDto cartItem)
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
