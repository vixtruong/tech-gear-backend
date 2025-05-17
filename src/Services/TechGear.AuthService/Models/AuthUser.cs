using System;
using System.Collections.Generic;

namespace TechGear.AuthService.Models;

public partial class AuthUser
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Email { get; set; } = null!;

    public string HashedPassword { get; set; } = null!;

    public string Role { get; set; } = null!;

    public bool IsActive { get; set; } = true;
}
