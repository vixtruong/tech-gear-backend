namespace TechGear.OrderService.DTOs
{
    public class OrderDetailDto
    {
        public int OrderId { get; set; }
        public string UserEmail { get; set; } = null!;
        public string RecipientName { get; set; } = null!;
        public string RecipientPhone { get; set; } = null!;
        public string Address { get; set; } = null!;
        public int? Point { get; set; } = null!;
        public string? CouponCode { get; set; }
        public decimal OrderTotalPrice { get; set; }
        public decimal PaymentTotalPrice { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string PaymentStatus { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public List<OrderItemDetailDto> OrderItems { get; set; } = new();
    }
}
