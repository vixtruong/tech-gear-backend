using TechGear.ProductService.Models;

namespace TechGear.ProductService.Interfaces
{
    public interface IProductConfigService
    {
        Task<IEnumerable<ProductConfiguration>> GetProductConfigurationsAsync();
        Task<IEnumerable<ProductConfiguration>> GetProductConfigurationsByProductItemIdAsync(int productItemId);
        Task<bool> AddProductConfigAsync(ProductConfiguration productConfiguration);
    }
}
