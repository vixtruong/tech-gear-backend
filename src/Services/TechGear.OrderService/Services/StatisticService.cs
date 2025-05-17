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

            return await GetDataBetweenRanges(start, endPrevious, startPrevious, endPrevious);
        }

        public async Task<IEnumerable<BestSellingDto>> GetBestSellingAsync()
        {
            try
            {
                var itemQuantities = await _context.OrderItems
                    .GroupBy(oi => oi.ProductItemId)
                    .Select(g => new
                    {
                        ProductItemId = g.Key,
                        TotalQuantity = g.Sum(x => x.Quantity)
                    })
                    .ToListAsync();

                var client = _httpClientFactory.CreateClient("ApiGatewayClient");
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

        // New methods for comparative revenue charts
        public async Task<ComparativeRevenueDto> GetAnnualRevenueComparisonAsync()
        {
            var currentYear = DateTime.Now.Year;
            var previousYear = currentYear - 1;

            var currentQuarters = new List<PeriodRevenue>();
            var previousQuarters = new List<PeriodRevenue>();

            // Calculate revenue for each quarter of the current year
            for (int quarter = 1; quarter <= 4; quarter++)
            {
                var start = new DateTime(currentYear, (quarter - 1) * 3 + 1, 1);
                var end = start.AddMonths(3);

                var revenue = await CalculateRevenueAsync(start, end);
                currentQuarters.Add(new PeriodRevenue
                {
                    PeriodName = $"Q{quarter}",
                    Revenue = revenue
                });
            }

            // Calculate revenue for each quarter of the previous year
            for (int quarter = 1; quarter <= 4; quarter++)
            {
                var start = new DateTime(previousYear, (quarter - 1) * 3 + 1, 1);
                var end = start.AddMonths(3);

                var revenue = await CalculateRevenueAsync(start, end);
                previousQuarters.Add(new PeriodRevenue
                {
                    PeriodName = $"Q{quarter}",
                    Revenue = revenue
                });
            }

            return new ComparativeRevenueDto
            {
                CurrentPeriod = currentQuarters,
                PreviousPeriod = previousQuarters
            };
        }

        public async Task<ComparativeRevenueDto> GetQuarterlyRevenueComparisonAsync()
        {
            var now = DateTime.Now;
            int currentQuarter = (now.Month - 1) / 3 + 1;

            var currentQuarterStart = new DateTime(now.Year, (currentQuarter - 1) * 3 + 1, 1);
            var previousQuarterStart = currentQuarterStart.AddMonths(-3);

            var currentMonths = new List<PeriodRevenue>();
            var previousMonths = new List<PeriodRevenue>();

            // Calculate revenue for each month of the current quarter
            for (int i = 0; i < 3; i++)
            {
                var start = currentQuarterStart.AddMonths(i);
                var end = start.AddMonths(1);

                var revenue = await CalculateRevenueAsync(start, end);
                currentMonths.Add(new PeriodRevenue
                {
                    PeriodName = start.ToString("MMM"),
                    Revenue = revenue
                });
            }

            // Calculate revenue for each month of the previous quarter
            for (int i = 0; i < 3; i++)
            {
                var start = previousQuarterStart.AddMonths(i);
                var end = start.AddMonths(1);

                var revenue = await CalculateRevenueAsync(start, end);
                previousMonths.Add(new PeriodRevenue
                {
                    PeriodName = start.ToString("MMM"),
                    Revenue = revenue
                });
            }

            return new ComparativeRevenueDto
            {
                CurrentPeriod = currentMonths,
                PreviousPeriod = previousMonths
            };
        }

        public async Task<ComparativeRevenueDto> GetMonthlyRevenueComparisonAsync()
        {
            var now = DateTime.Now;
            var currentMonthStart = new DateTime(now.Year, now.Month, 1);
            var previousMonthStart = currentMonthStart.AddMonths(-1);

            var currentWeeks = new List<PeriodRevenue>();
            var previousWeeks = new List<PeriodRevenue>();

            // Calculate revenue for each week of the current month
            var weekStart = currentMonthStart;
            int weekNumber = 1;
            while (weekStart < currentMonthStart.AddMonths(1))
            {
                var weekEnd = weekStart.AddDays(7) < currentMonthStart.AddMonths(1)
                    ? weekStart.AddDays(7)
                    : currentMonthStart.AddMonths(1);

                var revenue = await CalculateRevenueAsync(weekStart, weekEnd);
                currentWeeks.Add(new PeriodRevenue
                {
                    PeriodName = $"Week {weekNumber}",
                    Revenue = revenue
                });

                weekStart = weekEnd;
                weekNumber++;
            }

            // Calculate revenue for each week of the previous month
            weekStart = previousMonthStart;
            weekNumber = 1;
            while (weekStart < previousMonthStart.AddMonths(1))
            {
                var weekEnd = weekStart.AddDays(7) < previousMonthStart.AddMonths(1)
                    ? weekStart.AddDays(7)
                    : previousMonthStart.AddMonths(1);

                var revenue = await CalculateRevenueAsync(weekStart, weekEnd);
                previousWeeks.Add(new PeriodRevenue
                {
                    PeriodName = $"Week {weekNumber}",
                    Revenue = revenue
                });

                weekStart = weekEnd;
                weekNumber++;
            }

            return new ComparativeRevenueDto
            {
                CurrentPeriod = currentWeeks,
                PreviousPeriod = previousWeeks
            };
        }

        public async Task<ComparativeRevenueDto> GetWeeklyRevenueComparisonAsync()
        {
            var now = DateTime.Now;
            var startOfWeek = now.AddDays(-(int)now.DayOfWeek + 1); // Monday
            var currentWeekStart = startOfWeek.Date;
            var previousWeekStart = currentWeekStart.AddDays(-7);

            var currentDays = new List<PeriodRevenue>();
            var previousDays = new List<PeriodRevenue>();

            // Calculate revenue for each day of the current week
            for (int i = 0; i < 7; i++)
            {
                var dayStart = currentWeekStart.AddDays(i);
                var dayEnd = dayStart.AddDays(1);

                var revenue = await CalculateRevenueAsync(dayStart, dayEnd);
                currentDays.Add(new PeriodRevenue
                {
                    PeriodName = dayStart.ToString("ddd"),
                    Revenue = revenue
                });
            }

            // Calculate revenue for each day of the previous week
            for (int i = 0; i < 7; i++)
            {
                var dayStart = previousWeekStart.AddDays(i);
                var dayEnd = dayStart.AddDays(1);

                var revenue = await CalculateRevenueAsync(dayStart, dayEnd);
                previousDays.Add(new PeriodRevenue
                {
                    PeriodName = dayStart.ToString("ddd"),
                    Revenue = revenue
                });
            }

            return new ComparativeRevenueDto
            {
                CurrentPeriod = currentDays,
                PreviousPeriod = previousDays
            };
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

        private async Task<decimal> CalculateRevenueAsync(DateTime start, DateTime end)
        {
            try
            {
                var orders = await _context.Orders
                    .Where(o => o.CreateAt >= start && o.CreateAt < end)
                    .Include(o => o.Payments)
                    .ToListAsync();

                return orders.Sum(o => o.Payments.Sum(p => p.Amount));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CalculateRevenueAsync: {ex.Message}");
                return 0;
            }
        }
    }
}