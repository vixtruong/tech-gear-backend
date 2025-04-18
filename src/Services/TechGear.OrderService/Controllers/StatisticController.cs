﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechGear.OrderService.Interfaces;

namespace TechGear.OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticController(IStatisticService statisticService) : ControllerBase
    {
        private readonly IStatisticService _statisticService = statisticService;

        [HttpGet("best-seller-product-item-ids")]
        public async Task<IActionResult> GetBestSellerProductItemIds()
        {
            var productItemIds = await _statisticService.GetBestSellerProductItemIds();

            return Ok(productItemIds);
        }
    }
}
