namespace TechGear.AuthService.DTOs
{
    public class LogoutRequestDto
    {
        public int UserId { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
    }
}
