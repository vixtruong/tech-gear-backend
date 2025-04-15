namespace TechGear.OrderService.Interfaces
{
    public interface IStatisticService
    {
        Task<List<int>> GetBestSellerProductItemIds();
    }
}
