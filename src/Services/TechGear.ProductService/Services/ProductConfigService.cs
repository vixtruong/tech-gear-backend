using Microsoft.EntityFrameworkCore;
using TechGear.ProductService.Data;
using TechGear.ProductService.DTOs;
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

        public async Task<bool> AddProductConfigsAsync(List<ProductConfigDto> productConfiguration)
        {
            foreach (var config in productConfiguration)
            {
                var existConfig = await _context.ProductConfigurations
                    .FirstOrDefaultAsync(pc =>
                        pc.ProductItemId == config.ProductItemId &&
                        pc.VariationOptionId == config.VariationOptionId);

                if (existConfig != null)
                    continue;

                var entity = new ProductConfiguration
                {
                    ProductItemId = config.ProductItemId,
                    VariationOptionId = config.VariationOptionId,
                };

                _context.ProductConfigurations.Add(entity);
            }

            await _context.SaveChangesAsync();
            return true;
        }

    }
}
