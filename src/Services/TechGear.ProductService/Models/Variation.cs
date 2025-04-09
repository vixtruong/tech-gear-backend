using System;
using System.Collections.Generic;

namespace TechGear.ProductService.Models;

public partial class Variation
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<VariationOption> VariationOptions { get; set; } = new List<VariationOption>();
}
