using Microsoft.EntityFrameworkCore;
using TechGear.ProductService.Data;
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

        public async Task<ProductItem?> GetProductItemByIdAsync(int id)
        {
            return await _context.ProductItems.FirstOrDefaultAsync(pi => pi.Id == id);
        }

        public async Task<IEnumerable<ProductItem?>> GetProductItemsByProductIdAsync(int productId)
        {
            return await _context.ProductItems.Where(pi => pi.ProductId == productId).ToListAsync();
        }

        public async Task<ProductItem?> AddProductItemAsync(ProductItem productItem)
        {
            var existItem = await _context.ProductItems.FirstOrDefaultAsync(pi => pi.Sku == productItem.Sku);

            if (existItem != null) return null;

            _context.ProductItems.Add(productItem);
            await _context.SaveChangesAsync();

            return productItem;
        }

        public async Task<bool> UpdateProductItemAsync(ProductItem productItem)
        {
            var existItem = await _context.ProductItems.FindAsync(productItem.Id);

            if (existItem == null) return false;

            existItem.Sku = productItem.Sku;
            existItem.Price = productItem.Price;
            existItem.ProductId = productItem.ProductId;
            existItem.ProductImage = productItem.ProductImage;
            existItem.QtyInStock = productItem.QtyInStock;

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
