namespace TechGear.OrderService.Models
{
    public class Delivery
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Status { get; set; } = null!;
        public string? Note { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? ConfirmDate { get; set; }
        public DateTime? ShipDate { get; set; }
        public DateTime? CancelDate { get; set; }
        public string? CancelReason { get; set; }


        public virtual Order Order { get; set; } = null!;
    }
}
