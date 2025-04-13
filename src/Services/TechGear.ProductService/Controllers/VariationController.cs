using Microsoft.AspNetCore.Mvc;
using TechGear.ProductService.DTOs;
using TechGear.ProductService.Interfaces;
using TechGear.ProductService.Models;

namespace TechGear.ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VariationController : ControllerBase
    {
        private readonly IVariationService _variationService;

        public VariationController(IVariationService variationService)
        {
            _variationService = variationService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllVariations()
        {
            var variations = await _variationService.GetAllVariationsAsync();

            return Ok(variations);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetVariationById(int id)
        {
            var variation = await _variationService.GetVariationByIdAsync(id);
            if (variation == null)
            {
                return NotFound($"Variation with ID '{id}' not found.");
            }

            return Ok(variation);
        }

        [HttpGet("by-name/{name}")]
        public async Task<IActionResult> GetVariationByName(string name)
        {
            var variation = await _variationService.GetVariationByNameAsync(name);

            if (variation == null)
            {
                return NotFound($"Variation with name '{name}' not found.");
            }
            
            return Ok(variation);
        }

        [HttpGet("by-categoryId/{categoryId}")]
        public async Task<IActionResult> GetVariationsByCategoryId(int categoryId)
        {
            var variations = await _variationService.GetVariationsByCateIdAsync(categoryId);

            return Ok(variations);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddVariation([FromBody] VariationDto variation)
        {
            var newVariation = await _variationService.AddVariationAsync(variation);
            if (newVariation == null)
            {
                return BadRequest(new { message = "Variation already exists." });
            }

            return Ok(newVariation);
        }
    }
}
