using Microsoft.AspNetCore.Mvc;
using TechGear.ProductService.DTOs;
using TechGear.ProductService.Interfaces;
using TechGear.ProductService.Models;

namespace TechGear.ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductItemController : ControllerBase
    {
        private readonly IProductItemService _productItemService;

        public ProductItemController(IProductItemService productItemService)
        {
            _productItemService = productItemService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllProductItems()
        {
            var productItems = await _productItemService.GetAllProductItemsAsync();

            return Ok(productItems);
        }

        [HttpPost("get-by-ids")]
        public async Task<IActionResult> GetProductItemsInfo([FromBody] List<int>? ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("List ID invalid.");

            var productItemsInfo = await _productItemService.GetProductItemsByIdsAsync(ids);

            return Ok(productItemsInfo);
        }

        [HttpGet("by-productId/{productId}")]
        public async Task<IActionResult> GetProductItemsByProductId(int productId)
        {
            var productItems = await _productItemService.GetProductItemsByProductIdAsync(productId);

            return Ok(productItems);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductItemById(int id)
        {
            var productItem = await _productItemService.GetProductItemByIdAsync(id);
            if (productItem == null)
            {
                return NotFound($"Product item with ID {id} not found.");
            }

            return Ok(productItem);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddProductItem([FromBody] ProductItemDto item)
        {
            var newItem = await _productItemService.AddProductItemAsync(item);

            if (newItem == null)
            {
                return BadRequest(new { message = "Product item already exists." });
            }

            return Ok(newItem);
        }
    }
}
