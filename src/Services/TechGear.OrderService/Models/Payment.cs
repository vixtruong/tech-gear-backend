using System;
using System.Collections.Generic;

namespace TechGear.OrderService.Models;

public partial class Payment
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int Amount { get; set; }

    public string Method { get; set; } = null!;

    public DateTime? PaidAt { get; set; }

    public virtual Order Order { get; set; } = null!;
}
