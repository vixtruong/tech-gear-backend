﻿using System;
using System.Collections.Generic;

namespace TechGear.ProductService.Models;

public partial class Product
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public int BrandId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string ProductImage { get; set; } = null!;

    public DateTime CreateAt { get; set; }

    public bool Available { get; set; }

    public int Price { get; set; }

    public virtual Brand Brand { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<ProductItem> ProductItems { get; set; } = new List<ProductItem>();
}
