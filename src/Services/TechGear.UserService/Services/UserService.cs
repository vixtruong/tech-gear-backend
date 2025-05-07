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

        public async Task<TotalUserDto> GetTotalUserAsync()
        {
            var totalUsers = await _context.Users.CountAsync();
            var newUsers = await _context.Users
                .Where(u => u.CreatedAt >= DateTime.UtcNow.AddHours(7).AddDays(-30))
                .CountAsync();

            return new TotalUserDto
            {
                TotalUsers = totalUsers,
                NewUsers = newUsers,
            };
        }

        public async Task<UserDto?> AddUserAsync(UserDto user)
        {
            if (string.IsNullOrWhiteSpace(user.Email) ||
                string.IsNullOrWhiteSpace(user.FullName) ||
                string.IsNullOrWhiteSpace(user.PhoneNumber) ||
                string.IsNullOrWhiteSpace(user.Address))
            {
                throw new ArgumentException("Email, FullName, PhoneNumber và Address là bắt buộc.");
            }

            var exists = await _context.Users.AnyAsync(u => u.Email == user.Email);
            if (exists) return null;

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Tạo user mới
                var newUser = new User
                {
                    Email = user.Email,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                var newAddress = new UserAddress
                {
                    UserId = newUser.Id,
                    Address = user.Address,
                    RecipientName = user.FullName,
                    RecipientPhone = user.PhoneNumber,
                    IsDefault = true,
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                };

                _context.UserAddresses.Add(newAddress);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new UserDto
                {
                    Id = newUser.Id,
                    UserAddressId = newAddress.Id,
                    Email = newUser.Email,
                    FullName = newUser.FullName,
                    PhoneNumber = newUser.PhoneNumber,
                    Address = newAddress.Address,
                };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateUserAsync(EditProfileDto dto)
        {
            var user = await _context.Users.FindAsync(dto.Id);

            if (user == null)
            {
                return false;
            }

            user.FullName = dto.FullName;
            user.PhoneNumber = dto.PhoneNumber;
            user.Email = dto.Email;
            user.CreatedAt = DateTime.Now.AddHours(7);

            _context.Users.Update(user);
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<UserDto?> GetUserByIdAsync(int userId, int? userAddressId)
        {
            var user = await _context.Users
                .Include(u => u.UserAddresses)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Point = user.LoyaltyPoint,
                Address = user.UserAddresses
                    .Where(ua => ua.Id == userAddressId)
                    .Select(ua => ua.Address)
                    .FirstOrDefault(),
            };
        }

        public async Task<UserAddressInfoDto?> GetUserAddressByIdAsync(int userId, int? userAddressId)
        {
            var user = await _context.Users
                .Include(u => u.UserAddresses)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return null;

            var address = user.UserAddresses
                .Where(ua => ua.Id == userAddressId)
                .Select(ua => new UserAddressInfoDto
                {
                    Email = user.Email,
                    Address = ua.Address,
                    RecipientName = ua.RecipientName,
                    RecipientPhone = ua.RecipientPhone,
                })
                .FirstOrDefault();

            return address;
        }

        public async Task<string> GetUserNameAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user?.FullName ?? string.Empty;
        }

        public async Task<int?> GetPointAsync(int userId)
        {
            return await _context.Users.Where(u => u.Id == userId)
                .Select(u => u.LoyaltyPoint)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdatePointAsync(int userId, int orderId ,int usedPoint)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }

            if (user.LoyaltyPoint < usedPoint)
            {
                return false;
            }

            user.LoyaltyPoint -= usedPoint;

            var loyalty = new Loyalty
            {
                UserId = userId,
                FromOrderId = orderId,
                Point = -usedPoint,
                Action = "use",
                CreatedAt = DateTime.UtcNow.AddHours(7),
            };

            _context.Loyalties.Add(loyalty);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<List<string>> GetUsernameByUserIds(List<int> ids)
        {
            var usernames = await _context.Users
                .Where(u => ids.Contains(u.Id))
                .Select(u => u.FullName)
                .ToListAsync();

            return usernames;
        }
    }
}
