using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TechGear.AuthService.Data;
using TechGear.AuthService.Interfaces;
using TechGear.AuthService.Models;

namespace TechGear.AuthService.Services
{
    public class AuthService : IAuthService
    {
        private readonly TechGearAuthServiceContext _context;

        private readonly IConfiguration _configuration;

        public AuthService(TechGearAuthServiceContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthUser?> GetByUserIdAsync(int userId)
        {
            return await _context.AuthUsers.Where(u => u.Id == userId)
                .FirstOrDefaultAsync();
        }

        public async Task<AuthUser?> GetByUsernameAsync(string username)
        {
            return await _context.AuthUsers.Where(u => u.Email.ToLower().Equals(username.ToLower()))
                .FirstOrDefaultAsync();
        }

        public async Task<bool> RegisterAsync(AuthUser newAccount, string rawPassword)
        {
            if (await _context.AuthUsers.AnyAsync(a => a.Email == newAccount.Email)) return false;

            newAccount.HashedPassword = BCrypt.Net.BCrypt.HashPassword(rawPassword);
            _context.AuthUsers.Add(newAccount);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task RevokeRefreshTokenAsync(string token)
        {
            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.Token == token);

            if (refreshToken == null) return;

            refreshToken.IsRevoked = true;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsRefreshTokenValidAsync(string token)
        {
            return await _context.RefreshTokens.AnyAsync(t =>
                t.Token == token &&
                !t.IsRevoked &&
                t.ExpiryDate > DateTime.UtcNow);
        }

        public async Task<AuthUser?> ValidateCredentialsAsync(string email, string password)
        {
            var user = await _context.AuthUsers.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return null;

            var verified = BCrypt.Net.BCrypt.Verify(password, user.HashedPassword);

            return verified ? user : null;
        }

        public string GenerateAccessToken(AuthUser user)
        {
            var issuer = _configuration["JwtConfig:Issuer"];
            var audience = _configuration["JwtConfig:Audience"];
            var key = _configuration["JwtConfig:Key"];
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!));

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role),
            };

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(30),
                Audience = audience,
                Issuer = issuer,
                SigningCredentials = credentials
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);

            return handler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task StoreRefreshTokenAsync(int userId, string token)
        {
            var refreshToken = new RefreshToken
            {
                UserId = userId,
                Token = token,
                ExpiryDate = DateTime.UtcNow.AddDays(30)
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
        }
    }
}
