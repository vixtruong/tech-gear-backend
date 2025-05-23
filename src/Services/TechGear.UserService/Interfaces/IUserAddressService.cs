﻿using TechGear.UserService.DTOs;

namespace TechGear.UserService.Interfaces
{
    public interface IUserAddressService
    {
        Task<IEnumerable<UserAddressDto>?>  GetAddressesAsync(int userId);
        Task<UserAddressDto> AddUserAddressAsync(UserAddressDto dto);
        Task<bool> RemoveUserAddressAsync(int addressId);
        Task<bool> UpdateUserAddressAsync(UserAddressDto dto);
        Task<bool> SetDefaultAddressAsync(int userId, int addressId);
    }
}
