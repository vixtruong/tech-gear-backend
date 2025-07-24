namespace TechGear.UserService.DTOs
{
    public class UsePointDto
    {
        public int Point { get; set; }
        public int OrderId { get; set; }
        public string Action { get; set; } = "use";
    }
}
