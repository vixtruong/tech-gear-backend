using TechGear.ProductService.DTOs;
using TechGear.ProductService.Models;

namespace TechGear.ProductService.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product?>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int productId);
        Task<Product?> AddProductAsync(ProductDto product);
        Task<bool> UpdateProductAsync(ProductDto product);
        Task<bool> UpdateProductStatus(int id);
        Task<bool> DeleteProductAsync(int id);
        Task<bool> IsAvailableAsync(int productId);
    }
}
