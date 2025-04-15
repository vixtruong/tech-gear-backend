using Microsoft.EntityFrameworkCore;
using TechGear.OrderService.Data;
using TechGear.OrderService.Interfaces;

namespace TechGear.OrderService.Services
{
    public class StatisticService(TechGearOrderServiceContext context) : IStatisticService
    {
        private readonly TechGearOrderServiceContext _context = context;

        public async Task<List<int>> GetBestSellerProductItemIds()
        {
            return await _context.OrderItems
                .GroupBy(oi => oi.ProductItemId)
                .Select(g => new
                {
                    ProductItemId = g.Key,
                    TotalQuantity = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Select(x => x.ProductItemId)
                .ToListAsync();
        }
    }
}
