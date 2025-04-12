using Microsoft.AspNetCore.Mvc;
using TechGear.ProductService.Interfaces;
using TechGear.ProductService.Models;

namespace TechGear.ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllBrands()
        {
            var brands = await _brandService.GetBrandsAsync();

            return Ok(brands);
        }

        [HttpGet("by-name/{name}")]
        public async Task<IActionResult> GetBrandByName(string name)
        {
            var brand = await _brandService.GetBrandByNameAsync(name);
            if (brand == null)
                return NotFound($"Brand with name '{name}' not found");

            return Ok(brand);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetBrandById(int id)
        {
            var brand = await _brandService.GetBrandByIdAsync(id);
            if (brand == null)
                return NotFound($"Brand with ID {id} not found");

            return Ok(brand);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddBrand([FromBody] string name)
        {
            var added = await _brandService.AddBrandAsync(name);

            if (!added)
            {
                return BadRequest(new { message = $"Brand with name '{name}' already exists." });
            }

            return Ok(added);
        }
    }
}
