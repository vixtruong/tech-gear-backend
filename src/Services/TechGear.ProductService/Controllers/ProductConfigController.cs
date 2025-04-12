using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> AddProductConfig([FromBody] ProductConfiguration config)
        {
            var added = await _productConfigService.AddProductConfigAsync(config);
            if (!added)
            {
                return BadRequest(new { message = "Product config already exists." });
            }

            return Ok(added);
        }
    }
}
