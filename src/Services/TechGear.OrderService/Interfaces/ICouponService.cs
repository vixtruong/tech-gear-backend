using TechGear.OrderService.Models;

namespace TechGear.OrderService.Interfaces
{
    public interface ICouponService
    {
        Task<IEnumerable<Coupon>?> GetAllCouponsAsync();
        Task<Coupon?> GetCouponByCodeAsync(string code);
        Task<bool> IsCouponValidAsync(string code, decimal orderAmount);
        Task<bool> IsCouponExpiredAsync(string code);
        Task<bool> IsCouponUsageLimitReachedAsync(string code);
        Task<bool> CreateCouponAsync(Coupon coupon);
        Task<bool> UpdateCouponAsync(Coupon coupon);
        Task<bool> RemoveCouponUsageAsync(string code);

    }
}
