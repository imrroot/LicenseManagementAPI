﻿using LicenseManagementAPI.Application.Interfaces;
using LicenseManagementAPI.Presentation.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LicenseManagementAPI.Presentation.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

             return await _userService.RegisterUserAsync(registerDto);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            return await _userService.AuthenticateUserAsync(loginDto.Email, loginDto.Password);

        }
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = int.Parse(User.FindFirst("Id").Value);
            return await _userService.GetUserProfileAsync(userId);
        }
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserUpdateProfileDto updateProfileDto)
        {
            var userId = int.Parse(User.FindFirst("Id").Value);
            return await _userService.UpdateUserProfileAsync(userId, updateProfileDto);
        }

    }

}
