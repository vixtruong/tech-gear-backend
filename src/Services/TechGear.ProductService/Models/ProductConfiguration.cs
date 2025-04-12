using System;
using System.Collections.Generic;

namespace TechGear.ProductService.Models;

public partial class ProductConfiguration
{
    public int ProductItemId { get; set; }

    public int VariationOptionId { get; set; }

    public DateTime? CreateAt { get; set; }

    public virtual ProductItem ProductItem { get; set; } = null!;

    public virtual VariationOption VariationOption { get; set; } = null!;
}
