using Microsoft.EntityFrameworkCore;
using TechGear.ProductService.Data;
using TechGear.ProductService.Interfaces;
using TechGear.ProductService.Models;

namespace TechGear.ProductService.Services
{
    public class BrandService(TechGearProductServiceContext context) : IBrandService
    {
        private readonly TechGearProductServiceContext _context = context;


        public async Task<IEnumerable<Brand?>> GetBrandsAsync()
        {
            return await _context.Brands.ToListAsync();
        }

        public async Task<Brand?> GetBrandByNameAsync(string name)
        {
            return await _context.Brands
                .Where(b => b.Name.ToLower().Equals(name.ToLower()))
                .FirstOrDefaultAsync();
        }

        public async Task<Brand?> GetBrandByIdAsync(int id)
        {
            return await _context.Brands
                .Where(b => b.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> AddBrandAsync(string name)
        {
            var existBrand = await GetBrandByNameAsync(name);

            if (existBrand != null)
            {
                return false;
            }

            var brand = new Brand
            {
                Name = name
            };

            await _context.AddAsync(brand);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateBrandAsync(Brand brand)
        {
            var existBrand = await _context.Brands.FindAsync(brand.Id);

            if (existBrand == null)
            {
                return false;
            }

            existBrand.Name = brand.Name;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteBrandAsync(string name)
        {
            var existBrand = await GetBrandByNameAsync(name);

            if (existBrand == null)
            {
                return false;
            }

            _context.Brands.Remove(existBrand);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
