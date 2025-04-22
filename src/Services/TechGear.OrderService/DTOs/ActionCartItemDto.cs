namespace TechGear.OrderService.DTOs
{
    public class ActionCartItemDto
    {
        public int UserId { get; set; }
        public int ProductItemId { get; set; }
        public int? Quantity { get; set; } = 1;
    }
}
