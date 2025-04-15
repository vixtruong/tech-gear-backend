using System;
using System.Collections.Generic;

namespace TechGear.ProductService.Models;

public partial class ProductItem
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string Sku { get; set; } = null!;

    public int QtyInStock { get; set; }

    public string ProductImage { get; set; } = null!;

    public int Price { get; set; }

    public bool Available { get; set; }

    public DateTime CreateAt { get; set; }

    public int Discount { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual ICollection<ProductConfiguration> ProductConfigurations { get; set; } = new List<ProductConfiguration>();
}
