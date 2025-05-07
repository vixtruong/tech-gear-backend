namespace TechGear.UserService.DTOs
{
    public class UserAddressInfoDto
    {
        public string Email { get; set; } = null!;
        public string? RecipientName { get; set; }
        public string? RecipientPhone { get; set; }
        public string? Address { get; set; }
    }
}
