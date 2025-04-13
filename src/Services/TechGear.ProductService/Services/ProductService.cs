using Microsoft.EntityFrameworkCore;
using TechGear.ProductService.Data;
using TechGear.ProductService.DTOs;
using TechGear.ProductService.Interfaces;
using TechGear.ProductService.Models;

namespace TechGear.ProductService.Services
{
    public class ProductService(TechGearProductServiceContext context) : IProductService
    {
        private readonly TechGearProductServiceContext _context = context;

        public async Task<IEnumerable<Product?>> GetAllProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p!.Id == productId);
        }

        public async Task<Product?> AddProductAsync(ProductDto productDto)
        {
            if (await _context.Products.AnyAsync(p => p.Name == productDto.Name))
                return null;

            var entity = new Product
            {
                CategoryId = productDto.CategoryId,
                BrandId = productDto.BrandId,
                Name = productDto.Name,
                Description = productDto.Description,
                ProductImage = productDto.ProductImage,
                CreateAt = productDto.CreateAt,
                Available = productDto.Available,
                Price = productDto.Price
            };

            _context.Products.Add(entity);
            await _context.SaveChangesAsync();

            return entity;
        }


        public async Task<bool> UpdateProductAsync(ProductDto product)
        {
            var existProduct = await _context.Products.FindAsync(product.Id);

            if (existProduct == null) return false;

            existProduct.Name = product.Name;
            existProduct.Description = product.Description;
            existProduct.ProductImage = product.ProductImage;
            existProduct.CategoryId = product.CategoryId;
            existProduct.Price = product.Price;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateProductStatus(int id)
        {
            var existProduct = await _context.Products.FindAsync(id);

            if (existProduct == null) return false;

            existProduct.Available = !existProduct.Available;
            return true;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var existProduct = await _context.Products.FindAsync(id);

            if (existProduct == null) return false;

            _context.Products.Remove(existProduct);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsAvailableAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);

            if (product == null) return false;

            return product.Available;
        }
    }
}
