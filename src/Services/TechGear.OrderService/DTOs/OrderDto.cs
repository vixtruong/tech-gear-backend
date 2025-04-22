namespace TechGear.OrderService.DTOs
{
    public class OrderDto
    {
        public int? Id { get; set; }
        public int UserId { get; set; }
        public int UserAddressId { get; set; }
        public int? CouponId { get; set; }
        public bool? IsUsePoint { get; set; } = false;
        public int TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemDto>? OrderItems { get; set; }
    }

    public class OrderItemDto
    {
        public int? Id { get; set; }
        public int ProductItemId { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
    }
}
