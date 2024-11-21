﻿using LicenseManagementAPI.Core.Entities;

namespace LicenseManagementAPI.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserByIdAsync(int userId);
        Task<User> GetUserByEmailAsync(string email);
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
    }
}
