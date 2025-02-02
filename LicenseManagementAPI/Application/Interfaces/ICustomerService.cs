using LicenseManagementAPI.Application.Services;
using LicenseManagementAPI.Presentation.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using LicenseManagementAPI.Core.Entities;

namespace LicenseManagementAPI.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<IActionResult> CustomerValidateLicense(string licenseKey, string hwid, string ip,App app);
        Task<IActionResult> CustomerLoginAsync(EncryptedRequestDto encryptedRequest);
        Task<IActionResult> CustomerLicenseGetStatusAsync(EncryptedRequestDto encryptedRequest);
        Task<IActionResult> CustomerRenewLicenseAsync(EncryptedRequestDto encryptedRequest);
        Task<IActionResult> CustomerBanLicenseAsync(EncryptedRequestDto encryptedRequest);

    }
}
