using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechGear.OrderService.DTOs;
using TechGear.OrderService.Interfaces;

namespace TechGear.OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticController(IStatisticService statisticService) : ControllerBase
    {
        private readonly IStatisticService _statisticService = statisticService;

        [HttpGet("payments")]
        public async Task<IActionResult> GetPayments()
        {
            try
            {
                var payments = await _statisticService.GetPaymentsAsync();
                return Ok(payments);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpGet("best-seller-product-item-ids")]
        public async Task<IActionResult> GetBestSellerProductItemIds()
        {
            try
            {
                var productItemIds = await _statisticService.GetBestSellerProductItemIds();
                return Ok(productItemIds);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpGet("best-selling")]
        public async Task<IActionResult> GetBestSelling()
        {
            try
            {
                var result = await _statisticService.GetBestSellingAsync();
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpGet("annual")]
        public async Task<ActionResult<MockDataDto>> GetAnnualStats()
        {
            var result = await _statisticService.GetDataAnnuallyAsync();
            return Ok(result);
        }

        [HttpGet("quarter")]
        public async Task<ActionResult<MockDataDto>> GetQuarterStats()
        {
            var result = await _statisticService.GetDataQuarterlyAsync();
            return Ok(result);
        }

        [HttpGet("month")]
        public async Task<ActionResult<MockDataDto>> GetMonthlyStats()
        {
            var result = await _statisticService.GetDataMonthlyAsync();
            return Ok(result);
        }

        [HttpGet("week")]
        public async Task<ActionResult<MockDataDto>> GetWeeklyStats()
        {
            var result = await _statisticService.GetDataWeeklyAsync();
            return Ok(result);
        }

        [HttpGet("custom")]
        public async Task<ActionResult<MockDataDto>> GetCustomStats([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            if (start >= end)
                return BadRequest("Start date must be earlier than end date");

            var result = await _statisticService.GetDataCustomAsync(start, end);
            return Ok(result);
        }

        [HttpGet("annual-comparison")]
        public async Task<ActionResult<ComparativeRevenueDto>> GetAnnualRevenueComparison()
        {
            var result = await _statisticService.GetAnnualRevenueComparisonAsync();
            return Ok(result);
        }

        [HttpGet("quarter-comparison")]
        public async Task<ActionResult<ComparativeRevenueDto>> GetQuarterlyRevenueComparison()
        {
            var result = await _statisticService.GetQuarterlyRevenueComparisonAsync();
            return Ok(result);
        }

        [HttpGet("month-comparison")]
        public async Task<ActionResult<ComparativeRevenueDto>> GetMonthlyRevenueComparison()
        {
            var result = await _statisticService.GetMonthlyRevenueComparisonAsync();
            return Ok(result);
        }

        [HttpGet("week-comparison")]
        public async Task<ActionResult<ComparativeRevenueDto>> GetWeeklyRevenueComparison()
        {
            var result = await _statisticService.GetWeeklyRevenueComparisonAsync();
            return Ok(result);
        }
    }
}
