namespace TechGear.UserService.DTOs
{
    public class UserAddressDto
    {
        public int? Id { get; set; }
        public int UserId { get; set; }
        public string Address { get; set; } = null!;
        public string RecipientName { get; set; } = null!;
        public string RecipientPhone { get; set; } = null!;
        public bool IsDefault { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);
    }
}
