using Microsoft.EntityFrameworkCore;
using TechGear.ProductService.Data;
using TechGear.ProductService.Interfaces;
using TechGear.ProductService.Models;

namespace TechGear.ProductService.Services
{
    public class CategoryService(TechGearProductServiceContext context) : ICategoryService
    {
        private readonly TechGearProductServiceContext _context = context;

        public async Task<IEnumerable<Category?>> GetCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category?> GetCategoryByNameAsync(string name)
        {
            return await _context.Categories.Where(c => c.Name.ToLower().Equals(name.ToLower())).FirstOrDefaultAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories.Where(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> AddCategoryAsync(string name)
        {
            var existCate = await GetCategoryByNameAsync(name);

            if (existCate != null)
            {
                return false;
            }

            await _context.Categories.AddAsync(new Category { Name = name });
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateCategoryAsync(Category category)
        {
            var existCategory = await _context.Categories.FindAsync(category.Id);

            if (existCategory == null)
            {
                return false;
            }

            existCategory.Name = category.Name;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCategoryAsync(string name)
        {
            var existCategory = await GetCategoryByNameAsync(name);

            if (existCategory == null)
            {
                return false;
            }

            _context.Categories.Remove(existCategory);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
