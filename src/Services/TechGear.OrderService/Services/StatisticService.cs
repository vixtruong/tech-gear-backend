using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using TechGear.OrderService.Data;
using TechGear.OrderService.DTOs;
using TechGear.OrderService.Interfaces;
using TechGear.OrderService.Models;

namespace TechGear.OrderService.Services
{
    public class StatisticService(TechGearOrderServiceContext context, IHttpClientFactory httpClientFactory) : IStatisticService
    {
        private readonly TechGearOrderServiceContext _context = context;
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

        public async Task<IEnumerable<Payment>> GetPaymentsAsync()
        {
            return await _context.Payments.ToListAsync();
        }

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

                return bestSellers;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetBestSellerProductItemIds: {ex.Message}");
                return new List<int>();
            }
        }

        public async Task<MockDataDto> GetDataAnnuallyAsync()
        {
            var currentYear = DateTime.Now.Year;
            var previousYear = currentYear - 1;

            var startCurrent = new DateTime(currentYear, 1, 1);
            var endCurrent = startCurrent.AddYears(1);

            var startPrevious = new DateTime(previousYear, 1, 1);
            var endPrevious = startPrevious.AddYears(1);

            return await GetDataBetweenRanges(startCurrent, endCurrent, startPrevious, endPrevious);
        }

        public async Task<MockDataDto> GetDataQuarterlyAsync()
        {
            var now = DateTime.Now;
            int currentQuarter = (now.Month - 1) / 3 + 1;

            var startCurrent = new DateTime(now.Year, (currentQuarter - 1) * 3 + 1, 1);
            var endCurrent = startCurrent.AddMonths(3);

            var startPrevious = startCurrent.AddMonths(-3);
            var endPrevious = startCurrent;

            return await GetDataBetweenRanges(startCurrent, endCurrent, startPrevious, endPrevious);
        }

        public async Task<MockDataDto> GetDataMonthlyAsync()
        {
            var now = DateTime.Now;

            var startCurrent = new DateTime(now.Year, now.Month, 1);
            var endCurrent = startCurrent.AddMonths(1);

            var startPrevious = startCurrent.AddMonths(-1);
            var endPrevious = startCurrent;

            return await GetDataBetweenRanges(startCurrent, endCurrent, startPrevious, endPrevious);
        }

        public async Task<MockDataDto> GetDataWeeklyAsync()
        {
            var now = DateTime.Now;
            var startOfWeek = now.AddDays(-(int)now.DayOfWeek + 1); // Monday
            var startCurrent = startOfWeek.Date;
            var endCurrent = startCurrent.AddDays(7);

            var startPrevious = startCurrent.AddDays(-7);
            var endPrevious = startCurrent;

            return await GetDataBetweenRanges(startCurrent, endCurrent, startPrevious, endPrevious);
        }

        public async Task<MockDataDto> GetDataCustomAsync(DateTime start, DateTime end)
        {
            var previousPeriodLength = end - start;
            var startPrevious = start - previousPeriodLength;
            var endPrevious = start;

            return await GetDataBetweenRanges(start, end, startPrevious, endPrevious);
        }

        public async Task<IEnumerable<BestSellingDto>> GetBestSellingAsync()
        {
            try
            {
                // B1: Group theo ProductItemId để tính tổng quantity
                var itemQuantities = await _context.OrderItems
                    .GroupBy(oi => oi.ProductItemId)
                    .Select(g => new
                    {
                        ProductItemId = g.Key,
                        TotalQuantity = g.Sum(x => x.Quantity)
                    })
                    .ToListAsync();

                var client = _httpClientFactory.CreateClient("ApiGatewayClient");

                // B2: Fetch Category cho từng ProductItemId
                var categoryQuantities = new Dictionary<string, int>();

                foreach (var item in itemQuantities)
                {
                    try
                    {
                        var response = await client.GetAsync($"/api/v1/productItems/{item.ProductItemId}/category");
                        response.EnsureSuccessStatusCode();

                        var categoryName = await response.Content.ReadAsStringAsync();

                        if (!string.IsNullOrWhiteSpace(categoryName))
                        {
                            if (categoryQuantities.ContainsKey(categoryName))
                                categoryQuantities[categoryName] += item.TotalQuantity;
                            else
                                categoryQuantities[categoryName] = item.TotalQuantity;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Fail to get category for ProductItemId {item.ProductItemId}: {ex.Message}");
                    }
                }

                // B3: Sắp xếp và lấy top 10 category
                var result = categoryQuantities
                    .OrderByDescending(kv => kv.Value)
                    .Take(10)
                    .Select(kv => new BestSellingDto
                    {
                        Category = kv.Key,
                        SellingQuantity = kv.Value
                    })
                    .ToList();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetBestSellingAsync: {ex.Message}");
                return Enumerable.Empty<BestSellingDto>();
            }
        }


        private async Task<MockDataDto> GetDataBetweenRanges(
            DateTime startCurrent, DateTime endCurrent,
            DateTime startPrevious, DateTime endPrevious)
        {
            var currentOrders = await _context.Orders
                .Where(o => o.CreateAt >= startCurrent && o.CreateAt < endCurrent)
                .Include(o => o.Payments)
                .ToListAsync();

            var previousOrders = await _context.Orders
                .Where(o => o.CreateAt >= startPrevious && o.CreateAt < endPrevious)
                .Include(o => o.Payments)
                .ToListAsync();

            var currentRevenue = currentOrders.Sum(o => o.Payments.Sum(p => p.Amount));
            var previousRevenue = previousOrders.Sum(o => o.Payments.Sum(p => p.Amount));

            decimal growth = currentRevenue - previousRevenue;
            decimal growthPercent = previousRevenue == 0
                ? (currentRevenue > 0 ? 100 : 0)
                : (growth / previousRevenue) * 100;

            return new MockDataDto
            {
                TotalOrders = currentOrders.Count,
                Revenue = currentRevenue,
                Growth = growth,
                GrowthPercent = Math.Round(growthPercent, 2)
            };
        }

    }
}
