using Microsoft.EntityFrameworkCore;
using TechGear.ProductService.Data;
using TechGear.ProductService.Interfaces;
using TechGear.ProductService.Models;

namespace TechGear.ProductService.Services
{
    public class ProductConfigService(TechGearProductServiceContext context) : IProductConfigService
    {
        private readonly TechGearProductServiceContext _context = context;

        public async Task<IEnumerable<ProductConfiguration>> GetProductConfigurationsAsync()
        {
            return await _context.ProductConfigurations.ToListAsync();
        }

        public async Task<IEnumerable<ProductConfiguration>> GetProductConfigurationsByProductItemIdAsync(int productItemId)
        {
            return await _context.ProductConfigurations.Where(pc => pc.ProductItemId == productItemId).ToListAsync();
        }

        public async Task<bool> AddProductConfigAsync(ProductConfiguration productConfiguration)
        {
            var existConfig = await _context.ProductConfigurations
                .FirstOrDefaultAsync(pc => pc.ProductItemId == productConfiguration.ProductItemId
                                           && pc.VariationOptionId == productConfiguration.VariationOptionId);

            if (existConfig != null) return false;

            _context.ProductConfigurations.Add(productConfiguration);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
