using TechGear.ProductService.Models;

namespace TechGear.ProductService.Interfaces
{
    public interface IProductItemService
    {
        Task<IEnumerable<ProductItem?>> GetAllProductItemsAsync();
        Task<ProductItem?> GetProductItemByIdAsync(int id);
        Task<IEnumerable<ProductItem?>> GetProductItemsByProductIdAsync(int productId);
        Task<ProductItem?> AddProductItemAsync(ProductItem product);
        Task<bool> UpdateProductItemAsync(ProductItem product);
        Task<bool> UpdateProductItemStatus(int id);
        Task<bool> DeleteProductItemAsync(int id);
        Task<bool> IsAvailableAsync(int id);
    }
}
