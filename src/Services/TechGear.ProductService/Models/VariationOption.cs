using System;
using System.Collections.Generic;

namespace TechGear.ProductService.Models;

public partial class VariationOption
{
    public int Id { get; set; }

    public int VariationId { get; set; }

    public string Value { get; set; } = null!;

    public virtual Variation Variation { get; set; } = null!;

    public virtual ICollection<ProductItem> ProductItems { get; set; } = new List<ProductItem>();
}
