namespace TechGear.OrderService.DTOs
{
    public class CartListDto
    {
        public int UserId { get; set; }
        public List<int>? ProductItemIds { get; set; }
    }
}
