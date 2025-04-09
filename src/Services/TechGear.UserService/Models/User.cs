using System;
using System.Collections.Generic;

namespace TechGear.UserService.Models;

public partial class User
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? DeliveryAddress { get; set; }

    public virtual ICollection<Loyalty> Loyalties { get; set; } = new List<Loyalty>();
}
