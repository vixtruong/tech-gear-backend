namespace TechGear.UserService.DTOs
{
    public class AddLoyaltyPointsRequest
    {
        public int UserId { get; set; }
        public int Point { get; set; }
        public int OrderId { get; set; }
    }
}
