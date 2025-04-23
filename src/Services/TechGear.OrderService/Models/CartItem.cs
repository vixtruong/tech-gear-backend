using System;
using System.Collections.Generic;

namespace TechGear.OrderService.Models;

public partial class CartItem
{
    public int Id { get; set; }

    public int CartId { get; set; }

    public int ProductItemId { get; set; }

    public int Quantity { get; set; }

    public virtual Cart Cart { get; set; } = null!;
}
