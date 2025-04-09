using System;
using System.Collections.Generic;

namespace TechGear.ProductService.Models;

public partial class ProductReview
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int ProductId { get; set; }

    public string Content { get; set; } = null!;

    public int Rating { get; set; }

    public DateTime Timestamp { get; set; }

    public virtual Product Product { get; set; } = null!;
}
