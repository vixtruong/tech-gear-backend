﻿using System;
using System.Collections.Generic;

namespace TechGear.ProductService.Models;

public partial class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<Variation> Variations { get; set; } = new List<Variation>();
}
