namespace TechGear.ProductService.DTOs
{
    public class ProductItemDto
    {
        public int Id { get; set; }
        public string Sku { get; set; } = string.Empty;
        public int QtyInStock { get; set; }
        public string ProductImage { get; set; } = string.Empty;
        public int Price { get; set; }
        public bool Available { get; set; }
        public DateTime CreateAt { get; set; }
        public int ProductId { get; set; }
    }
}