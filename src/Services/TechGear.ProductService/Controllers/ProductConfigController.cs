using Microsoft.AspNetCore.Mvc;
using TechGear.ProductService.DTOs;
using TechGear.ProductService.Interfaces;
using TechGear.ProductService.Models;

namespace TechGear.ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductConfigController : ControllerBase
    {
        private readonly IProductConfigService _productConfigService;

        public ProductConfigController(IProductConfigService productConfigService)
        {
            _productConfigService = productConfigService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllProductConfigs()
        {
            var configs = await _productConfigService.GetProductConfigurationsAsync();

            return Ok(configs);
        }

        [HttpGet("by-productItemId/{productItemId}")]
        public async Task<IActionResult> GetConfigsByProductItemId(int productItemId)
        {
            var configs = await _productConfigService.GetProductConfigurationsByProductItemIdAsync(productItemId);

            return Ok(configs);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddProductConfigs([FromBody] List<ProductConfigDto> configs)
        {
            var added = await _productConfigService.AddProductConfigsAsync(configs);

            if (!added)
                return BadRequest(new { message = "Duplicate config found." });

            return Ok(added);
        }
    }
}
