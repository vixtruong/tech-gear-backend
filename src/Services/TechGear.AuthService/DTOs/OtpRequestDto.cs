namespace TechGear.AuthService.DTOs
{
    public class OtpRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
    }
}
