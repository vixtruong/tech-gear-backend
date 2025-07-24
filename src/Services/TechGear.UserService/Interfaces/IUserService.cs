using TechGear.UserService.DTOs;

namespace TechGear.UserService.Interfaces
{
    public interface IUserService
    {
        Task<TotalUserDto> GetTotalUserAsync();
        Task<UserDto?> AddUserAsync(UserDto user);
        Task<bool> UpdateUserAsync(EditProfileDto dto);
        Task<UserDto?> GetUserByIdAsync(int userId, int? userAddressId);
        Task<UserAddressInfoDto?> GetUserAddressByIdAsync(int userId, int? userAddressId);
        Task<string> GetUserNameAsync(int userId);
        Task<int?> GetPointAsync(int userId);
        Task<bool> UpdatePointAsync(int userId, int orderId, int usedPoint, string action);
        Task<List<string>> GetUsernameByUserIds(List<int> ids);
    }
}
