﻿using System.ComponentModel.DataAnnotations;

namespace LicenseManagementAPI.Presentation.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
    public class RegisterDto
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

    }

    public class UserProfileDto
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string OwnerId {get; set; }
        public DateTime RegistrationDate { get; set; }
    }

    public class UserUpdateProfileDto
    {
        public string Username {get; set; }
        public string Password {get; set; }
    }
    
}
