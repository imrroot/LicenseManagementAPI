using LicenseManagementAPI.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace LicenseManagementAPI.Presentation.DTOs
{
    public class LicenseResponeDto
    {
        public string LicenseKey { get; set; }
        public string Note { get; set; }
        public LicenseStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastUsedDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime? FreezeStartTime { get; set; }
        public DateTime? FreezeEndTime { get; set; }
        public string HWID { get; set; }
        public string IP { get; set; }
        
    }
    public class BanLicenseDto
    {
        [Required(ErrorMessage = "License ID is required")]
        public int LicenseId { get; set; }

        public string Reason { get; set; }
    }
    public class CreateLicenseDto
    {
        [Required(ErrorMessage = "Application ID is required")]
        public int ApplicationId { get; set; }

        [StringLength(100)]
        public string Notes { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Pattern must be up to 20 characters.")]
        public string? Pattern { get; set; } = "****-****-****-****";

        [Required(ErrorMessage = "Access Level is required")]
        public string Subscription { get; set; }

        [Required]
        public ExpiryUnit ExpiryUnit { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Duration must be positive.")]
        public int Duration { get; set; }

        public string? InitialHWID { get; set; }
        public string? InitialIP { get; set; }
    }
    public class DeleteLicenseDto
    {
        [Required(ErrorMessage = "License ID is required")]
        public int LicenseId { get; set; }
    }
    public class EncryptedRequestDto
    {
        public string AppKey { get; set; }
        public string EncryptedData { get; set; }
    }
    public class FreezeLicenseDto
    {
        [Required(ErrorMessage = "License ID is required")]
        public int LicenseId { get; set; }

        public bool IsFrozen { get; set; }
    }
    public class LicenseLoginDto
    {
        [Required(ErrorMessage = "License key is required")]
        public string LicenseKey { get; set; }

        [Required(ErrorMessage = "HWID is required")]
        public string HWID { get; set; }

        public string IP { get; set; }
    }
    public class LicenseStatusDto
    {
        public string Key { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string ApplicationName { get; set; }
        public string LicenseStatus { get; set; }
    }
    public class RenewLicenseDto
    {
        public string LicenseKey { get; set; }
    }
}
