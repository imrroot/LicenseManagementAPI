﻿using LicenseManagementAPI.Presentation.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace LicenseManagementAPI.Application.Interfaces
{
    public interface IUserService
    {
        Task<IActionResult> RegisterUserAsync(RegisterDto registerDto);
        Task<IActionResult> AuthenticateUserAsync(string email, string password);
        Task<IActionResult> GetUserProfileAsync(int userId);
        Task<IActionResult> UpdateUserProfileAsync(int userId,UserUpdateProfileDto  updateProfileDto);
    }


}
