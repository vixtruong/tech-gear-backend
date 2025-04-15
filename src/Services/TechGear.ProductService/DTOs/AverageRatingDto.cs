namespace TechGear.ProductService.DTOs
{
    public class AverageRatingDto
    {
        public int ProductId { get; set; }

        public double AverageRating { get; set; }

        public int RatingCount { get; set; }
    }
}
