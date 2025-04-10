namespace TechGear.AuthService.DTOs
{
    public class RefreshTokenRequestDto
    {
        public int UserId { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}
