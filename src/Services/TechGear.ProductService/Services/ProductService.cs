using Microsoft.EntityFrameworkCore;
using TechGear.ProductService.Data;
using TechGear.ProductService.DTOs;
using TechGear.ProductService.Interfaces;
using TechGear.ProductService.Models;

namespace TechGear.ProductService.Services
{
    public class ProductService(TechGearProductServiceContext context, IHttpClientFactory httpClientFactory) : IProductService
    {
        private readonly TechGearProductServiceContext _context = context;
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

        public async Task<IEnumerable<Product?>> GetAllProductsForAdminAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<IEnumerable<Product?>> GetAllProductsAsync()
        {
            return await _context.Products.Where(p => p.Available).ToListAsync();
        }

        public async Task<IEnumerable<Product?>> GetPromotionProductsAsync()
        {
            var products = await _context.Products
                .Where(p => p.Available)
                .Include(p => p.ProductItems)
                .ToListAsync();

            foreach (var product in products)
            {
                product.ProductItems = product.ProductItems
                    .Where(pi => pi.Discount > 0)
                    .ToList();
            }

            return products.Where(p => p.ProductItems.Any()).ToList();
        }



        public async Task<IEnumerable<Product?>> GetNewProductsAsync()
        {
            var thresholdDate = DateTime.Now.AddDays(-14);

            return await _context.Products
                .Where(p => p.CreateAt >= thresholdDate && p.Available)
                .ToListAsync();
        }


        public async Task<IEnumerable<ProductDto?>> GetBestSellerProductsAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiGatewayClient");

            var response = await client.GetAsync("api/v1/statistics/best-seller-product-item-ids");

            if (!response.IsSuccessStatusCode)
            {
                return new List<ProductDto?>();
            }

            var productItemIds = await response.Content.ReadFromJsonAsync<List<int>>();

            if (productItemIds == null)
            {
                return new List<ProductDto?>();
            }

            var productItems = await _context.ProductItems
                .Include(pi => pi.Product)
                .Where(pi => productItemIds.Contains(pi.Id))
                .ToListAsync();

            var orderedProducts = productItemIds
                .Select(id => productItems.FirstOrDefault(pi => pi.Id == id)?.Product)
                .Where(p => p != null)
                .Distinct() 
                .ToList();

            var result = orderedProducts
                .Where(p => p!.Available)
                .Select(p => new ProductDto
                {
                    Id = p!.Id,
                    Name = p.Name,
                    BrandId = p.BrandId,
                    CategoryId = p.CategoryId,
                    Description = p.Description,
                    ProductImage = p.ProductImage,
                    CreateAt = p.CreateAt,
                    Available = p.Available,
                    Price = p.Price
                })
                .ToList();

            return result;
        }

        public async Task<IEnumerable<ProductDto?>> GetProductsByIdsAsync(List<int> ids)
        {
            var products = await _context.Products
                .Where(p => ids.Contains(p.Id) && p.Available)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    BrandId = p.BrandId,
                    CategoryId = p.CategoryId,
                    Description = p.Description,
                    ProductImage = p.ProductImage,
                    CreateAt = p.CreateAt,
                    Available = p.Available,
                    Price = p.Price
                })
                .ToListAsync();

            return products;
        }

        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p!.Id == productId && p.Available);
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
                CreateAt = DateTime.Now.AddHours(7),
                Available = productDto.Available,
                Price = productDto.Price,
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
            existProduct.BrandId = product.BrandId;
            existProduct.Price = product.Price;

            await _context.SaveChangesAsync();
            return true;
        }

        

        public async Task<bool> ToggleProductAvailable(int id)
        {
            var existProduct = await _context.Products.FindAsync(id);

            if (existProduct == null) return false;

            existProduct.Available = !existProduct.Available;
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
