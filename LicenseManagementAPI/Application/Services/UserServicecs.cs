using System.IdentityModel.Tokens.Jwt;
using LicenseManagementAPI.Application.Interfaces;
using LicenseManagementAPI.Core.Entities;
using LicenseManagementAPI.Infrastructure.Interfaces;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using LicenseManagementAPI.Presentation.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace LicenseManagementAPI.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<IActionResult> RegisterUserAsync(RegisterDto registerDto)
        {
            if (await _userRepository.GetUserByEmailAsync(registerDto.Email) != null)
                return new BadRequestObjectResult(new {Message = "Email already exists"});

            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password)
            };

            await _userRepository.AddUserAsync(user);
            var token = GenerateJwtToken(user);
            return new OkObjectResult(new {Message = "Registration successful",Token = token });
        }

        public async Task<IActionResult> AuthenticateUserAsync(string email, string password)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return new BadRequestObjectResult(new { Message = "Invalid credentials" });

            var token = GenerateJwtToken(user);
            return new OkObjectResult(new { Message = "Login successful", Token = token });
        }


        public async Task<IActionResult> GetUserProfileAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) return new NotFoundObjectResult(new { Message = "User not found." });
            var profile = new UserProfileDto
            {
                Email = user.Email,
                Username = user.Username,
                OwnerId = user.OwnerId,
                RegistrationDate = user.RegistrationDate
            };
            return new OkObjectResult(new {Profile = profile});
        }

        public async Task<IActionResult> UpdateUserProfileAsync(int userId, UserUpdateProfileDto updateProfileDto)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) return new NotFoundObjectResult(new { Message = "User not found." });

            if (!string.IsNullOrEmpty(updateProfileDto.Username))
                user.Username = updateProfileDto.Username;

            if (!string.IsNullOrEmpty(updateProfileDto.Password))
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateProfileDto.Password); // Ensure password hashing

            await _userRepository.UpdateUserAsync(user);
            return new OkObjectResult(new { Message = "Profile updated successfully." });
        }




        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: new[]
                {
                    new Claim("Id", user.UserId.ToString()),
                    new Claim("Email", user.Email)
                },
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        
    }

}
