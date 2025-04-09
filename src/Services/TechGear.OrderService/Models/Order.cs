using System;
using System.Collections.Generic;

namespace TechGear.OrderService.Models;

public partial class Order
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int TotalAmount { get; set; }

    public DateTime CreateAt { get; set; }

    public int? CouponId { get; set; }

    public virtual Coupon? Coupon { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
