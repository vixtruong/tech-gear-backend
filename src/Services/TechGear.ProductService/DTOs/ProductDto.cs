namespace TechGear.ProductService.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }

        public int BrandId { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string ProductImage { get; set; } = null!;

        public DateTime CreateAt { get; set; }

        public bool Available { get; set; }

        public int Price { get; set; }
    }
}
