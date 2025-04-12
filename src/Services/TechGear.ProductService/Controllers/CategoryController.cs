using Microsoft.AspNetCore.Mvc;
using TechGear.ProductService.Interfaces;

namespace TechGear.ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
       private readonly ICategoryService _categoryService;

       public CategoryController(ICategoryService categoryService)
       {
           _categoryService = categoryService;
       }

       [HttpGet("all")]
       public async Task<IActionResult> GetAllCategories()
       {
           var categories = await _categoryService.GetCategoriesAsync();

           return Ok(categories);
       }

       [HttpGet("by-name/{name}")]
       public async Task<IActionResult> GetCategoryByName(string name)
       {
           var cate = await _categoryService.GetCategoryByNameAsync(name);
           if (cate == null)
           {
               return NotFound($"Category with name '{name}' not found.");
           }

           return Ok(cate);
       }

       [HttpGet("{id:int}")]
       public async Task<IActionResult> GetCategoryById(int id)
       {
           var cate = await _categoryService.GetCategoryByIdAsync(id);
           if (cate == null)
           {
               return NotFound($"Category with ID '{id}' not found.");
           }

           return Ok(cate);
       }

       [HttpPost("add")]
       public async Task<IActionResult> AddCategory([FromBody] string name)
       {
           var added = await _categoryService.AddCategoryAsync(name);

           if (!added)
           {
               return BadRequest(new { message = $"Category with name '{name} already exists.'" });
           }

           return Ok(added);
       }
    }
}
