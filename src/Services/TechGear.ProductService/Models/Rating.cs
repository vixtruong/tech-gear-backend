using System;
using System.Collections.Generic;

namespace TechGear.ProductService.Models;

public partial class Rating
{
    public int Id { get; set; }

    public int ProductItemId { get; set; }

    public int OrderId { get; set; }

    public int UserId { get; set; }

    public int Star { get; set; }

    public string? Content { get; set; }

    public DateTime LastUpdate { get; set; }

    public virtual ProductItem ProductItem { get; set; } = null!;
}
