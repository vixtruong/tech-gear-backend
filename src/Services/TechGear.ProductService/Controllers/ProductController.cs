using Microsoft.AspNetCore.Mvc;
using TechGear.ProductService.DTOs;
using TechGear.ProductService.Interfaces;
using TechGear.ProductService.Models;

namespace TechGear.ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IProductService productService) : ControllerBase
    {
        private readonly IProductService _productService = productService;

        [HttpGet("all")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();

            return Ok(products);
        }

        [HttpGet("best-sellers")]
        public async Task<IActionResult> GetBestSellerProducts()
        {
            var products = await _productService.GetBestSellerProductsAsync();

            return Ok(products);
        }

        [HttpGet("new")]
        public async Task<IActionResult> GetNewProducts()
        {
            var products = await _productService.GetNewProductsAsync();

            return Ok(products);
        }

        [HttpGet("promotions")]
        public async Task<IActionResult> GetPromotionProducts()
        {
            var products = await _productService.GetPromotionProductsAsync();

            return Ok(products);
        }

        [HttpGet]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound($"Product with ID '{id}' not found.");
            }

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] ProductDto product)
        {
            var newProduct = await _productService.AddProductAsync(product);

            if (newProduct == null)
            {
                return BadRequest(new { message = "Product already exists." });
            }

            return Ok(newProduct);
        }
    }
}
