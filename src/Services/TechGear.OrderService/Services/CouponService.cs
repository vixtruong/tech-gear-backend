using Microsoft.EntityFrameworkCore;
using TechGear.OrderService.Data;
using TechGear.OrderService.Interfaces;
using TechGear.OrderService.Models;

namespace TechGear.OrderService.Services
{
    public class CouponService(TechGearOrderServiceContext context) : ICouponService
    {
        private readonly TechGearOrderServiceContext _context = context;

        public async Task<IEnumerable<Coupon>?> GetAllCouponsAsync()
        {
            var coupons = await _context.Coupons
                .AsNoTracking()
                .ToListAsync();

            return coupons;
        }

        public async Task<Coupon?> GetCouponByCodeAsync(string code)
        {
            var coupon = await _context.Coupons
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Code == code);

            return coupon;
        }

        public async Task<bool> IsCouponValidAsync(string code, decimal orderAmount)
        {
            var coupon = await GetCouponByCodeAsync(code);
            if (coupon == null)
            {
                return false;
            }

            if (coupon.ExpirationDate.HasValue && coupon.ExpirationDate.Value < DateTime.UtcNow.AddHours(7))
            {
                return false;
            }

            if (coupon.UsageLimit <= 0)
            {
                return false;
            }

            if (orderAmount < coupon.MinimumOrderAmount)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> IsCouponExpiredAsync(string code)
        {
            var coupon = await GetCouponByCodeAsync(code);
            if (coupon == null)
            {
                return false;
            }

            if (coupon.ExpirationDate.HasValue && coupon.ExpirationDate.Value < DateTime.UtcNow.AddHours(7))
            {
                return true;
            }

            return false;
        }

        public async Task<bool> IsCouponUsageLimitReachedAsync(string code)
        {
            var coupon = await GetCouponByCodeAsync(code);
            if (coupon == null)
            {
                return false;
            }

            return coupon.UsageLimit <= 0;
        }

        public async Task<bool> CreateCouponAsync(Coupon? coupon)
        {
            if (coupon == null)
            {
                return false;
            }

            await _context.Coupons.AddAsync(coupon);
            var result = await _context.SaveChangesAsync();

            return result > 0;
        }

        public async Task<bool> UpdateCouponAsync(Coupon coupon)
        {
            var existingCoupon = await GetCouponByCodeAsync(coupon.Code);
            if (existingCoupon == null)
            {
                return false;
            }

            _context.Coupons.Update(coupon);
            var result = await _context.SaveChangesAsync();

            return result > 0;
        }

        public async Task<bool> RemoveCouponUsageAsync(string code)
        {
            var coupon = await GetCouponByCodeAsync(code);
            if (coupon == null || coupon.UsageLimit <= 0)
            {
                return false;
            }

            coupon.UsageLimit--;

            if (coupon.UsageLimit <= 0)
            {
                _context.Coupons.Remove(coupon);
            }
            else
            {
                _context.Coupons.Update(coupon);
            }

            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
    }
}
