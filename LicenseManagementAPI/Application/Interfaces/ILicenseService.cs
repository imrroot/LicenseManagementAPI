using LicenseManagementAPI.Presentation.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace LicenseManagementAPI.Application.Interfaces
{
    public interface ILicenseService
    {
        
        Task<IActionResult> FreezeLicenseAsync(string licenseKey,int userId);
        Task<IActionResult> UnfreezeLicenseAsync(string licenseKey,int userId);
        Task<IActionResult> CreateLicenseAsync(CreateLicenseDto createLicenseDto,int userId);
        Task<IActionResult> BanLicenseAsync(string licenseKey,int userId);
        Task<IActionResult> GetLicenseStatusAsync(string licenseKey,int userId);
        Task<IActionResult> DeleteLicenseAsync(string licenseKey,int userId);
        Task<IActionResult> CustomerValidateLicense(string licenseKey, string hwid, string ip);
        Task<IActionResult> CustomerLoginAsync(EncryptedRequestDto encryptedRequest);
        Task<IActionResult> GetCustomerLicenseStatusAsync(EncryptedRequestDto encryptedRequest);
        Task<IActionResult> CustomerRenewLicenseAsync(EncryptedRequestDto encryptedRequest);
        Task<IActionResult> CustomerBanLicenseAsync(EncryptedRequestDto encryptedRequest);
        
    }


}
