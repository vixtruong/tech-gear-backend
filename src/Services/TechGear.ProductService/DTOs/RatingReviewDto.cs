namespace TechGear.ProductService.DTOs
{
    public class RatingReviewDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductItemId { get; set; }
        public string ImgUrl { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public string Sku { get; set; } = null!;
        public int UserId { get; set; }
        public int Star { get; set; }
        public string? Content { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
