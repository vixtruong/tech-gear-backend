﻿namespace TechGear.AuthService.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }

        public int? UserAddressId { get; set; }

        public string Email { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public string? Address { get; set; } = null!;
    }
}
