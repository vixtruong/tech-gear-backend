﻿namespace TechGear.AuthService.DTOs
{
    public class RegisterRequestDto
    {
        public string Email { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string? PhoneNumber { get; set; } = null!;

        public string? DeliveryAddress { get; set; } = null!;

        public string RawPassword { get; set; } = null!;

        public string Role { get; set; } = null!;
    }
}
