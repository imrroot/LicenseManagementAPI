using System.Net;
using LicenseManagementAPI.Application.Interfaces;
using LicenseManagementAPI.Presentation.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace LicenseManagementAPI.Presentation.Controllers
{
    [Route("api/customer")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ILicenseService _licenseService;

        public CustomerController(ILicenseService licenseService)
        {
            _licenseService = licenseService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] EncryptedRequestDto request)
        {
            return  await _licenseService.CustomerLoginAsync(request);
            
        }

        [HttpPost("status")]
        public async Task<IActionResult> CheckStatus([FromBody] EncryptedRequestDto request)
        {
            return  await _licenseService.GetCustomerLicenseStatusAsync(request);
            
        }

        [HttpPost("renew")]
        public async Task<IActionResult> Renew([FromBody] EncryptedRequestDto request)
        {
            return await _licenseService.CustomerRenewLicenseAsync(request);
        }

        [HttpPost("ban")]
        public async Task<IActionResult> Ban([FromBody] EncryptedRequestDto request)
        {
            return await _licenseService.CustomerBanLicenseAsync(request);
        }
    }

}
