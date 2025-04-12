using Microsoft.AspNetCore.Mvc;
using TechGear.ProductService.Interfaces;
using TechGear.ProductService.Models;

namespace TechGear.ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VariationOptionController : ControllerBase
    {
        private readonly IVariationOptionService _variationOptionService;

        public VariationOptionController(IVariationOptionService variationOptionService)
        {
            _variationOptionService = variationOptionService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllVariationOptions()
        {
            var varOptions = await _variationOptionService.GetAllVariationOptionsAsync();

            return Ok(varOptions);
        }

        [HttpGet("by-variationId/{variationId}")]
        public async Task<IActionResult> GetVariationOptionsByVariationId(int variationId)
        {
            var varOptions = await _variationOptionService.GetAllVariationOptionsByVariationIdAsync(variationId);

            return Ok(varOptions);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetVariationOptionById(int id)
        {
            var option = await _variationOptionService.GetVariationOptionByIdAsync(id);
            if (option == null)
            {
                return NotFound($"Variation option with ID '{id}' not found.");
            }

            return Ok(option);
        }

        [HttpGet("by-value/{value}")]
        public async Task<IActionResult> GetVariationOptionByValue(string value)
        {
            var option = await _variationOptionService.GetVariationOptionByValueAsync(value);
            if (option == null)
            {
                return NotFound($"Variation option with Value '{value}' not found.");
            }

            return Ok(option);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddVariationOption(VariationOption option)
        {
            var newOption = await _variationOptionService.AddVariationOptionAsync(option);
            if (newOption == null)
            {
                return BadRequest(new { message = "Variation option already exists." });
            }
            
            return Ok(newOption);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteVariationOption(int id)
        {
            var deleted = await _variationOptionService.RemoveVariationOptionAsync(id);
            if (!deleted)
            {
                return NotFound($"Variation option with id '{id}' not found.'");
            }

            return NoContent();
        }
    }
}
