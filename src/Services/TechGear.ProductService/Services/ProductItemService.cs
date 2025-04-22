using Microsoft.EntityFrameworkCore;
using TechGear.ProductService.Data;
using TechGear.ProductService.DTOs;
using TechGear.ProductService.Interfaces;
using TechGear.ProductService.Models;

namespace TechGear.ProductService.Services
{
    public class ProductItemService(TechGearProductServiceContext context) : IProductItemService
    {
        private readonly TechGearProductServiceContext _context = context;

        public async Task<IEnumerable<ProductItem?>> GetAllProductItemsAsync()
        {
            return await _context.ProductItems.ToListAsync();
        }

        public async Task<IEnumerable<ProductItemInfoDto?>> GetProductItemsByIdsAsync(List<int>? ids)
        {
            if (ids == null || !ids.Any())
                return Enumerable.Empty<ProductItemInfoDto>();

            var productItems = await _context.ProductItems
                .Where(p => ids.Contains(p.Id))
                .Select(p => new ProductItemInfoDto
                {
                    ProductItemId = p.Id,
                    ProductName = p.Product.Name,
                    Sku = p.Sku,
                    ImageUrl = p.Product.ProductImage,
                    Price = p.Price,
                    Discount = p.Discount,
                })
                .ToListAsync();

            return productItems;
        }

        public async Task<ProductItem?> GetProductItemByIdAsync(int id)
        {
            return await _context.ProductItems.FirstOrDefaultAsync(pi => pi.Id == id);
        }

        public async Task<List<int>?> GetPriceAsync(List<int> ids)
        {
            return await _context.ProductItems
                .Where(pi => ids.Contains(pi.Id))
                .Select(pi => pi.Price)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductItem?>> GetProductItemsByProductIdAsync(int productId)
        {
            return await _context.ProductItems.Where(pi => pi.ProductId == productId).ToListAsync();
        }

        public async Task<ProductItem?> AddProductItemAsync(ProductItemDto productItem)
        {
            var existItem = await _context.ProductItems.FirstOrDefaultAsync(pi => pi.Sku == productItem.Sku);
            if (existItem != null) return null;

            var entity = new ProductItem
            {
                Sku = productItem.Sku,
                QtyInStock = productItem.QtyInStock,
                ProductImage = productItem.ProductImage,
                Price = productItem.Price,
                Available = productItem.Available,
                CreateAt = productItem.CreateAt,
                ProductId = productItem.ProductId,
                Discount = productItem.Discount ?? 0,
            };

            _context.ProductItems.Add(entity);
            await _context.SaveChangesAsync();

            return entity;
        }


        public async Task<bool> UpdateProductItemAsync(ProductItemDto productItem)
        {
            var existItem = await _context.ProductItems.FindAsync(productItem.Id);

            if (existItem == null) return false;

            existItem.Sku = productItem.Sku;
            existItem.Price = productItem.Price;
            existItem.ProductId = productItem.ProductId;
            existItem.ProductImage = productItem.ProductImage;
            existItem.QtyInStock = productItem.QtyInStock;
            existItem.Discount = productItem.Discount ?? existItem.Discount;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateProductItemStatus(int id)
        {
            var existItem = await _context.ProductItems.FindAsync(id);

            if (existItem == null) return false;

            existItem.Available = !existItem.Available;

            return true;
        }

        public async Task<bool> DeleteProductItemAsync(int id)
        {
            var existItem = await _context.ProductItems.FindAsync(id);

            if (existItem == null) return false;

            _context.ProductItems.Remove(existItem);
            return true;
        }

        public async Task<bool> IsAvailableAsync(int id)
        {
            var existItem = await _context.ProductItems.FindAsync(id);

            if (existItem == null) return false;

            return existItem.Available;
        }
    }
}
