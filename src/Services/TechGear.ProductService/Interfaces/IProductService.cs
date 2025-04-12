using TechGear.ProductService.Models;

namespace TechGear.ProductService.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product?>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int productId);
        Task<Product?> AddProductAsync(Product product);
        Task<bool> UpdateProductAsync(Product product);
        Task<bool> UpdateProductStatus(int id);
        Task<bool> DeleteProductAsync(int id);
        Task<bool> IsAvailableAsync(int productId);
    }
}
