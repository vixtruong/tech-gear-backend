using System;
using System.Collections.Generic;

namespace TechGear.UserService.Models;

public partial class Loyalty
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int FromOrderId { get; set; }

    public int Point { get; set; }

    public string Action { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
