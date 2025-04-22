namespace TechGear.OrderService.DTOs
{
    public class OrderEmailDto
    {
        public string CustomerName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string? Note { get; set; }

        public List<OrderItemEmailDto> Items { get; set; } = new List<OrderItemEmailDto>();
        public int UsedPoints { get; set; }
        public int DiscountValue { get; set; }
    }

    public class OrderItemEmailDto
    {
        public string ProductName { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
