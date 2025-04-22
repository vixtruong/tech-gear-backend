using Microsoft.AspNetCore.Mvc;
using TechGear.AuthService.DTOs;
using TechGear.AuthService.Interfaces;
using TechGear.AuthService.Models;

namespace TechGear.AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthController(IAuthService authService, IHttpClientFactory httpClientFactory)
        {
            _authService = authService;
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto user)
        {
            // Call api from UserService to create User then create Account with userId
            var client = _httpClientFactory.CreateClient("ApiGatewayClient");

            var userDto = new UserDto
            {
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
            };

            Console.WriteLine(userDto.Email + "-" + userDto.FullName + "-" + userDto.PhoneNumber + "-" + userDto.Address);

            var response = await client.PostAsJsonAsync("api/v1/users/create", userDto);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, new { message = "User already exist." });
            }

            var createdUser = await response.Content.ReadFromJsonAsync<UserDto>();

            if (createdUser == null)
            {
                return StatusCode(500, new { message = "Failed to parse user creation response." });
            }

            // Create account with user created before
            var newAccount = new AuthUser
            {
                UserId = createdUser.Id,
                Email = user.Email,
                Role = user.Role
            };

            var success = await _authService.RegisterAsync(newAccount, user.RawPassword ?? createdUser.PhoneNumber);

            if (!success) return BadRequest(new { message = "An account with this email already exists." });

            return Ok(new { userId = newAccount.UserId, userAddressId = createdUser.UserAddressId });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var verifiedUser = await _authService.ValidateCredentialsAsync(request.Email, request.Password);

            if (verifiedUser == null) return Unauthorized(new
            {
                message = "Login failed. Please check your email and password."
            });

            var accessToken = _authService.GenerateAccessToken(verifiedUser);
            var refreshToken = _authService.GenerateRefreshToken();

            await _authService.StoreRefreshTokenAsync(verifiedUser.UserId, refreshToken);

            return Ok(new {
                message = "Login successful.",
                accessToken = accessToken,
                refreshToken = refreshToken
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDto request)
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
            {
                return BadRequest(new { message = "RefreshToken required." });
            }

            var valid = await _authService.IsRefreshTokenValidAsync(request.RefreshToken);

            if (!valid)
            {
                return Unauthorized(new { message = "Invalid or expired refresh token." });
            }

            await _authService.RevokeRefreshTokenAsync(request.RefreshToken);

            return Ok(new { message = "Logout successful." });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            var valid = await _authService.IsRefreshTokenValidAsync(request.Token);

            if (!valid)
            {
                return Unauthorized(new { message = "Invalid or expired refresh token." });
            }

            var user = await _authService.GetByUserIdAsync(request.UserId);

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var newAccessToken = _authService.GenerateAccessToken(user);
            var newRefreshToken = _authService.GenerateRefreshToken();

            await _authService.RevokeRefreshTokenAsync(request.Token);
            await _authService.StoreRefreshTokenAsync(user.UserId, newRefreshToken);

            return Ok(new
            {
                accessToken = newAccessToken,
                refreshToken = newRefreshToken,
                message = "Token refreshed successfully."
            });
        }
    }
}
