namespace TechGear.OrderService.DTOs
{
    public class OrderStatusDto
    {
        public int OrderId { get; set; }
        public string Status { get; set; } = null!;
        public string? CancelReason { get; set; }
    }
}
