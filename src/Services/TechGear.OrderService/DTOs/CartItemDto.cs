namespace TechGear.OrderService.DTOs
{
    public class CartItemDto
    {
        public int ProductItemId { get; set; }
        public int? Quantity { get; set; }

        public string ProductName { get; set; }
        public string Sku { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
    }
}
