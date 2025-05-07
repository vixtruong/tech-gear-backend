using TechGear.OrderService.DTOs;
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
        Task<bool> CreateCouponAsync(CouponDto coupon);
        Task<bool> UpdateCouponAsync(CouponDto coupon);
        Task<bool> DeleteCouponAsync(int id);
        Task<bool> RemoveCouponUsageAsync(string code);

    }
}
