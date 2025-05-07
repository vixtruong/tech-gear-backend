using TechGear.ProductService.DTOs;
using TechGear.ProductService.Models;

namespace TechGear.ProductService.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product?>> GetAllProductsForAdminAsync();
        Task<IEnumerable<Product?>> GetAllProductsAsync();
        Task<IEnumerable<Product?>> GetPromotionProductsAsync();
        Task<IEnumerable<Product?>> GetNewProductsAsync();
        Task<IEnumerable<ProductDto?>> GetBestSellerProductsAsync();
        Task<IEnumerable<ProductDto?>> GetProductsByIdsAsync(List<int> ids);
        Task<Product?> GetProductByIdAsync(int productId);
        Task<Product?> AddProductAsync(ProductDto product);
        Task<bool> UpdateProductAsync(ProductDto product);
        Task<bool> ToggleProductAvailable(int id);
        Task<bool> IsAvailableAsync(int productId);
    }
}
