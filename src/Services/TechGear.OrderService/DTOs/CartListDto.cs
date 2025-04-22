namespace TechGear.OrderService.DTOs
{
    public class CartListDto
    {
        public int UserId { get; set; }
        public List<CartItemDto>? CartItems { get; set; }
    }
}
