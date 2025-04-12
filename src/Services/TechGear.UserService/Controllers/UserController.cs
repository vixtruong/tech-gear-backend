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

    }
}
