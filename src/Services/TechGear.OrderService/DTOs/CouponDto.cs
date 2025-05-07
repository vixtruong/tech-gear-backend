namespace TechGear.OrderService.DTOs
{
    public class CouponDto
    {
        public int Id { get; set; }

        public string Code { get; set; } = null!;

        public int Value { get; set; }

        public int UsageLimit { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public int MinimumOrderAmount { get; set; }
    }
}
