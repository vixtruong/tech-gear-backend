using Microsoft.AspNetCore.Mvc;
using TechGear.UserService.DTOs;
using TechGear.UserService.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TechGear.UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController(IUserAddressService userAddressService) : ControllerBase
    {
        private readonly IUserAddressService _userAddressService = userAddressService;

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserAddresses(int userId)
        {
            var addresses = await _userAddressService.GetAddressesAsync(userId);

            return Ok(addresses);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddUserAddress([FromBody] UserAddressDto dto)
        {
            var added = await _userAddressService.AddUserAddressAsync(dto);

            return CreatedAtAction(nameof(GetUserAddresses), new { userId = dto.UserId }, added);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUserAddress([FromBody] UserAddressDto dto)
        {
            var updated = await _userAddressService.UpdateUserAddressAsync(dto);

            if (!updated)
            {
                return NotFound(new { message = "Address not found" });
            }

            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUserAddress(int id)
        {
            var deleted = await _userAddressService.RemoveUserAddressAsync(id);

            if (!deleted)
            {
                return NotFound(new { message = "Address not found" });
            }

            return NoContent();
        }
    }
}
