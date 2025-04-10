using TechGear.ProductService.Models;

namespace TechGear.ProductService.Interfaces
{
    public interface ICategoryService
    {
        public Task<IEnumerable<Category?>> GetCategoriesAsync();
        public Task<Category?> GetCategoryByNameAsync(string name);
        public Task<Category?> GetCategoryByIdAsync(int id);
        public Task<bool> AddCategoryAsync(string name);
        public Task<bool> UpdateCategoryAsync(Category category);
        public Task<bool> DeleteCategoryAsync(string name);
    }
}
