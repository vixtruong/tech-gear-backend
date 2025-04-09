using System;
using System.Collections.Generic;

namespace TechGear.OrderService.Models;

public partial class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int ProductItemId { get; set; }

    public int Quantity { get; set; }

    public int Price { get; set; }

    public virtual Order Order { get; set; } = null!;
}
