using System;
using System.Collections.Generic;

namespace TechGear.UserService.Models;

public partial class User
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public int LoyaltyPoint { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Loyalty> Loyalties { get; set; } = new List<Loyalty>();

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual ICollection<UserAddress> UserAddresses { get; set; } = new List<UserAddress>();
}
