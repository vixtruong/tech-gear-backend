using TechGear.UserService.DTOs;

namespace TechGear.UserService.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> AddUserAsync(UserDto user);
        Task<bool> UpdateUserAsync(EditProfileDto dto);
        Task<UserDto?> GetUserByIdAsync(int userId, int? userAddressId);
        Task<string> GetUserNameAsync(int userId);
        Task<int?> GetPointAsync(int userId);
        Task<bool> UpdatePointAsync(int userId, int orderId ,int usedPoint);
        Task<List<string>> GetUsernameByUserIds(List<int> ids);
    }
}
