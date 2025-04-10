using TechGear.UserService.DTOs;

namespace TechGear.UserService.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> AddUserAsync(UserDto user);
    }
}
