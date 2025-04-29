namespace TechGear.AuthService.Models
{
    public class Otp
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string Code { get; set; } = string.Empty;
        public DateTime ExpiryTime { get; set; }
        public bool IsUsed { get; set; }
    }
}