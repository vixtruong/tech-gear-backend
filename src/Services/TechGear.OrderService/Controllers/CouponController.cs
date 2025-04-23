using Microsoft.AspNetCore.Mvc;
using TechGear.OrderService.Interfaces;
using TechGear.OrderService.Models;

namespace TechGear.OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponController(ICouponService couponService) : ControllerBase
    {
        private readonly ICouponService _couponService = couponService;

        [HttpGet]
        public async Task<IActionResult> GetAllCoupons()
        {
            var coupons = await _couponService.GetAllCouponsAsync();
            if (coupons != null && coupons.Any())
            {
                return Ok(coupons);
            }
            return NotFound("No coupons found.");
        }

        //[HttpGet("validate/{code}/{orderAmount}")]
        //public async Task<IActionResult> ValidateCoupon(string code, decimal orderAmount)
        //{
        //    var isValid = await _couponService.IsCouponValidAsync(code, orderAmount);
        //    if (isValid)
        //    {
        //        return Ok(new { IsValid = true });
        //    }
        //    return BadRequest(new { IsValid = false });
        //}

        //[HttpGet("expired/{code}")]
        //public async Task<IActionResult> IsCouponExpired(string code)
        //{
        //    var isExpired = await _couponService.IsCouponExpiredAsync(code);
        //    if (isExpired)
        //    {
        //        return Ok(new { IsExpired = true });
        //    }
        //    return BadRequest(new { IsExpired = false });
        //}

        //[HttpGet("usage-limit/{code}")]
        //public async Task<IActionResult> IsCouponUsageLimitReached(string code)
        //{
        //    var isLimitReached = await _couponService.IsCouponUsageLimitReachedAsync(code);
        //    if (isLimitReached)
        //    {
        //        return Ok(new { IsLimitReached = true });
        //    }
        //    return BadRequest(new { IsLimitReached = false });
        //}

        [HttpPost("create")]
        public async Task<IActionResult> CreateCoupon([FromBody] Coupon coupon)
        {
            if (ModelState.IsValid)
            {
                var result = await _couponService.CreateCouponAsync(coupon);
                if (result)
                {
                    return CreatedAtAction(nameof(GetCouponByCode), new { code = coupon.Code }, coupon);
                }
                return BadRequest("Failed to create coupon.");
            }
            return BadRequest(ModelState);
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetCouponByCode(string code)
        {
            var coupon = await _couponService.GetCouponByCodeAsync(code);
            if (coupon != null)
            {
                return Ok(coupon);
            }
            return NotFound();
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateCoupon([FromBody] Coupon coupon)
        {
            if (ModelState.IsValid)
            {
                var result = await _couponService.UpdateCouponAsync(coupon);
                if (result)
                {
                    return Ok(coupon);
                }
                return BadRequest("Failed to update coupon.");
            }
            return BadRequest(ModelState);
        }

        [HttpDelete("remove-usage/{code}")]
        public async Task<IActionResult> RemoveCouponUsage(string code)
        {
            var result = await _couponService.RemoveCouponUsageAsync(code);
            if (result)
            {
                return Ok(new { Message = "Coupon usage removed successfully." });
            }
            return BadRequest("Failed to remove coupon usage.");
        }
    }
}
