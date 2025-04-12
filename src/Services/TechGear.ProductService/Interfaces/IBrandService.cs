
using TechGear.ProductService.Models;

namespace TechGear.ProductService.Interfaces
{
    public interface IBrandService
    {
        public Task<IEnumerable<Brand?>> GetBrandsAsync();
        public Task<Brand?> GetBrandByNameAsync(string name);
        public Task<Brand?> GetBrandByIdAsync(int id);
        public Task<bool> AddBrandAsync(string name);
        public Task<bool> UpdateBrandAsync(Brand brand);
        public Task<bool> DeleteBrandAsync(string name);
    }
}
