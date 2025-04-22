namespace TechGear.OrderService.DTOs
{
    public class UserInfoResponse
    {
        public int Id { get; set; }

        public int? UserAddressId { get; set; }

        public string Email { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public int? Point { get; set; }

        public string? Address { get; set; }
    }
}
