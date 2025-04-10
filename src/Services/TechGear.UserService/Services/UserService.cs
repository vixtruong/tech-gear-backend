using Microsoft.EntityFrameworkCore;
using TechGear.UserService.Data;
using TechGear.UserService.DTOs;
using TechGear.UserService.Interfaces;
using TechGear.UserService.Models;

namespace TechGear.UserService.Services
{
    public class UserService : IUserService
    {
        private readonly TechGearUserServiceContext _context;

        public UserService(TechGearUserServiceContext context)
        {
            _context = context;
        }

        public async Task<UserDto?> AddUserAsync(UserDto user)
        {
            var exists = await _context.Users.AnyAsync(u => u.Email == user.Email);
            if (exists) return null;

            var newUser = new User
            {
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                DeliveryAddress = user.DeliveryAddress
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Id = newUser.Id,
                Email = newUser.Email,
                FullName = newUser.FullName,
                PhoneNumber = newUser.PhoneNumber,
                DeliveryAddress = newUser.DeliveryAddress
            };
        }

    }
}
