using System;
using System.Collections.Generic;

namespace TechGear.OrderService.Models;

public partial class Coupon
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public int Value { get; set; }

    public int UsageLimit { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
