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
            try
            {
                var bestSellers = await _context.OrderItems
                    .GroupBy(oi => oi.ProductItemId)
                    .Select(g => new
                    {
                        ProductItemId = g.Key,
                        TotalQuantity = g.Sum(x => x.Quantity)
                    })
                    .OrderByDescending(x => x.TotalQuantity)
                    .Select(x => x.ProductItemId)
                    .ToListAsync();

                return bestSellers ?? new List<int>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetBestSellerProductItemIds: {ex.Message}");
                return new List<int>();
            }
        }
    }
}
