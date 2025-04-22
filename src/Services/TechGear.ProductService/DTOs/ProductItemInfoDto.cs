namespace TechGear.ProductService.DTOs
{
    public class ProductItemInfoDto
    {
        public int ProductItemId { get; set; }
        public string ProductName { get; set; } = "";
        public string Sku { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public decimal Price { get; set; }
        public int Discount { get; set; }
    }
}
