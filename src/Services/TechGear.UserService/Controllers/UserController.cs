using Microsoft.AspNetCore.Mvc;
using TechGear.UserService.DTOs;
using TechGear.UserService.Interfaces;

namespace TechGear.UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] UserDto user)
        {
            var result = await _userService.AddUserAsync(user);

            if (result == null)
            {
                return Conflict(new { message = "User already exists." });
            }

            return Ok(result);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(int userId, [FromQuery] int? userAddressId)
        {
            var user = await _userService.GetUserByIdAsync(userId, userAddressId);

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(user);
        }

        [HttpGet("{userId}/name")]
        public async Task<IActionResult> GetUserName(int userId)
        {
            var userName = await _userService.GetUserNameAsync(userId);

            if (string.IsNullOrEmpty(userName))
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(userName);
        }

        [HttpGet("{userId}/points")]
        public async Task<IActionResult> UserPoints(int userId)
        {
            var points = await _userService.GetPointAsync(userId);

            if (points == null)
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(points);
        }

        [HttpPut("{userId}/points")]
        public async Task<IActionResult> UpdateUserPoints(int userId, [FromBody] int usedPoint)
        {
            var success = await _userService.UpdatePointAsync(userId, usedPoint);

            if (!success)
            {
                return BadRequest(new { message = "Failed to update points." });
            }

            return Ok(new { message = "Points updated successfully." });
        }

        [HttpPost("user-names")]
        public async Task<IActionResult> GetUsernameByIds([FromBody] List<int> ids)
        {
            var userNames = await _userService.GetUsernameByUserIds(ids);

            return Ok(userNames);
        }
    }
}
