using TechGear.ProductService.DTOs;
using TechGear.ProductService.Models;

namespace TechGear.ProductService.Interfaces
{
    public interface IProductItemService
    {
        Task<IEnumerable<ProductItem?>> GetAllProductItemsAsync();
        Task<IEnumerable<ProductItemInfoDto?>> GetProductItemsByIdsAsync(List<int>? ids);
        Task<ProductItem?> GetProductItemByIdAsync(int id);
        Task<IEnumerable<ProductItem?>> GetProductItemsByProductIdAsync(int productId);
        Task<ProductItem?> AddProductItemAsync(ProductItemDto product);
        Task<bool> UpdateProductItemAsync(ProductItemDto product);
        Task<bool> UpdateProductItemStatus(int id);
        Task<bool> DeleteProductItemAsync(int id);
        Task<bool> IsAvailableAsync(int id);
    }
}
