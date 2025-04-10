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
        Task<bool> IsRefreshTokenValidAsync(string token);
        Task<AuthUser?> GetByUsernameAsync(string username);
        Task<AuthUser?> GetByUserIdAsync(int userId);
        Task<bool> RegisterAsync(AuthUser newAccount, string rawPassword);
    }
}
