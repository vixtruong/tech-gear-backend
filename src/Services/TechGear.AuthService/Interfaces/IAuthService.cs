using TechGear.AuthService.DTOs;
using TechGear.AuthService.Models;

namespace TechGear.AuthService.Interfaces
{
    public interface IAuthService
    {
        Task<AuthUser?> ValidateCredentialsAsync(string email, string password);
        string GenerateAccessToken(AuthUser user);
        string GenerateRefreshToken();
        Task StoreRefreshTokenAsync(int userId, string token);
        Task RevokeRefreshTokenAsync(string token);
        Task<bool> IsRefreshTokenValidAsync(int userId, string token);
        Task<bool> IsOtpCodeValidAsync(string email, string otp);
        Task<AuthUser?> GetByEmailAsync(string email);
        Task<AuthUser?> GetByUserIdAsync(int userId);
        Task<bool> RegisterAsync(AuthUser newAccount, string rawPassword);
        Task<int> GenerateOtpCodeAsync(string email);
        Task<bool> ResetPasswordAsync(ResetPasswordDto dto);
        Task<bool> ChangePasswordAsync(ChangePasswordDto dto);
    }
}