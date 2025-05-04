using Microsoft.AspNetCore.Mvc;
using TechGear.UserService.DTOs;
using TechGear.UserService.Interfaces;
using TechGear.UserService.Models;

namespace TechGear.UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoyaltyController(ILoyaltyService loyaltyService) : ControllerBase
    {
        private readonly ILoyaltyService _loyaltyService = loyaltyService;

        [HttpPost("add")]
        public async Task<IActionResult> AddLoyaltyPoints([FromBody] AddLoyaltyPointsRequest request)
        {
            var success = await _loyaltyService.AddLoyaltyPointsAsync(request.UserId, request.OrderId, request.Point, request.Action);
            if (!success)
            {
                return BadRequest(new { message = "Failed to add loyalty points." });
            }
            return Ok(new { message = "Loyalty points added successfully." });
        }

        [HttpGet("{userId}/all")]
        public async Task<IActionResult> GetAllLoyaltiesByUserId(int userId)
        {
            var loyalties = await _loyaltyService.GetAllLoyaltiesByUserIdAsync(userId);
            return Ok(loyalties);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLoyaltyById(int id)
        {
            var loyalty = await _loyaltyService.GetLoyaltyByIdAsync(id);
            if (loyalty == null)
            {
                return NotFound();
            }
            return Ok(loyalty);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateLoyalty([FromBody] Loyalty loyalty)
        {
            var success = await _loyaltyService.AddLoyaltyPointsAsync(loyalty.UserId, loyalty.FromOrderId, loyalty.Point, loyalty.Action);
            if (!success)
            {
                return BadRequest(new { message = "Failed to create loyalty." });
            }
            return CreatedAtAction(nameof(GetLoyaltyById), new { id = loyalty.Id }, loyalty);
        }
    }
}
