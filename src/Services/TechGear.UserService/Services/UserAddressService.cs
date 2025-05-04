using Microsoft.EntityFrameworkCore;
using TechGear.UserService.Data;
using TechGear.UserService.DTOs;
using TechGear.UserService.Interfaces;
using TechGear.UserService.Models;

namespace TechGear.UserService.Services
{
    public class UserAddressService(TechGearUserServiceContext context) : IUserAddressService
    {
        private readonly TechGearUserServiceContext _context = context;

        public async Task<IEnumerable<UserAddressDto>?> GetAddressesAsync(int userId)
        {
            return await _context.UserAddresses.Where(u => u.UserId == userId)
                .Select(u => new UserAddressDto
            {
                Id = u.Id,
                UserId = u.UserId,
                Address = u.Address,
                RecipientName = u.RecipientName,
                RecipientPhone = u.RecipientPhone,
                IsDefault = u.IsDefault,
                CreatedAt = u.CreatedAt,
            }).ToListAsync();
        }

        public async Task<UserAddressDto> AddUserAddressAsync(UserAddressDto dto)
        {
            var exist = await _context.UserAddresses.AnyAsync(u => u.UserId == dto.UserId);

            var newAddress = new UserAddress
            {
                UserId = dto.UserId,
                Address = dto.Address,
                RecipientName = dto.RecipientName,
                RecipientPhone = dto.RecipientPhone,
                IsDefault = (!exist),
                CreatedAt = dto.CreatedAt,
            };

            _context.UserAddresses.Add(newAddress);
            await _context.SaveChangesAsync();

            return new UserAddressDto
            {
                Id = newAddress.Id,
                UserId = newAddress.UserId,
                Address = newAddress.Address,
                RecipientName = newAddress.RecipientName,
                RecipientPhone = newAddress.RecipientPhone,
                IsDefault = newAddress.IsDefault,
                CreatedAt = newAddress.CreatedAt
            };
        }

        public async Task<bool> RemoveUserAddressAsync(int addressId)
        {
            var address = await _context.UserAddresses.FindAsync(addressId);

            if (address == null)
            {
                return false;
            }

            _context.UserAddresses.Remove(address);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateUserAddressAsync(UserAddressDto dto)
        {
            var address = await _context.UserAddresses.FindAsync(dto.Id);

            if (address == null)
            {
                return false;
            }

            address.Address = dto.Address;
            address.RecipientName = dto.RecipientName;
            address.RecipientPhone = dto.RecipientPhone;

            _context.UserAddresses.Update(address);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SetDefaultAddressAsync(int userId, int addressId)
        {
            var address = await _context.UserAddresses.FindAsync(addressId);

            if (address == null || address.UserId != userId)
            {
                return false;
            }

            // Set tất cả các địa chỉ của user về false
            var allUserAddresses = await _context.UserAddresses
                .Where(u => u.UserId == userId && u.IsDefault)
                .ToListAsync();

            foreach (var addr in allUserAddresses)
            {
                addr.IsDefault = false;
            }

            // Gán địa chỉ mới là mặc định
            address.IsDefault = true;

            await _context.SaveChangesAsync();

            return true;
        }

    }
}
