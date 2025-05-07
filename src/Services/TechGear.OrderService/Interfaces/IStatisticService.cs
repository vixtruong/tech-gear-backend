using TechGear.OrderService.DTOs;
using TechGear.OrderService.Models;

namespace TechGear.OrderService.Interfaces
{
    public interface IStatisticService
    {
        Task<IEnumerable<Payment>> GetPaymentsAsync();
        Task<List<int>> GetBestSellerProductItemIds();
        Task<MockDataDto> GetDataAnnuallyAsync();
        Task<MockDataDto> GetDataQuarterlyAsync();
        Task<MockDataDto> GetDataMonthlyAsync();
        Task<MockDataDto> GetDataWeeklyAsync();
        Task<MockDataDto> GetDataCustomAsync(DateTime start, DateTime end);
        Task<IEnumerable<BestSellingDto>> GetBestSellingAsync();
    }
}
