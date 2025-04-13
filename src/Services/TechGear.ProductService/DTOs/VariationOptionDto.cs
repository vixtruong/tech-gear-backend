namespace TechGear.ProductService.DTOs
{
    public class VariationOptionDto
    {
        public int Id { get; set; }

        public int VariationId { get; set; }

        public string Value { get; set; } = null!;
    }
}
