using System;
using System.Collections.Generic;

namespace TechGear.ProductService.Models;

public partial class ProductItem
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string Sku { get; set; } = null!;

    public int QtyInStock { get; set; }

    public string? Description { get; set; }

    public string? ProductImage { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual ICollection<VariationOption> VariationOptions { get; set; } = new List<VariationOption>();
}
