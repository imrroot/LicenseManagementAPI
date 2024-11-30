using LicenseManagementAPI.Application.Interfaces;
using LicenseManagementAPI.Presentation.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LicenseManagementAPI.Presentation.Controllers
{
    [Authorize]
    [Route("api/license")]
    [ApiController]
    public class LicenseController : ControllerBase
    {
        private readonly ILicenseService _licenseService;

        public LicenseController(ILicenseService licenseService)
        {
            _licenseService = licenseService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateLicense([FromBody] CreateLicenseDto createLicenseDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var userId = int.Parse(User.FindFirst("Id").Value); 
           return await _licenseService.CreateLicenseAsync(createLicenseDto,userId);
            
        }

        [HttpPost("list/{appId}")]
        public async Task<IActionResult> GetLicenseList(int appId)
        {
           var userId = int.Parse(User.FindFirst("Id").Value);
           return await _licenseService.GetLicensesAppAsync(appId, userId);
            
        }

        [HttpGet("status/{licenseKey}")]
        public async Task<IActionResult> CheckStatus(string licenseKey)
        {
            var userId = int.Parse(User.FindFirst("Id").Value); 
            return await _licenseService.GetLicenseStatusAsync(licenseKey,userId);
        }

        [HttpPost("ban/{licenseKey}")]
        public async Task<IActionResult> BanLicense(string licenseKey)
        {
            var userId = int.Parse(User.FindFirst("Id").Value);
            return await _licenseService.BanLicenseAsync(licenseKey, userId);
        }

        [HttpPost("freeze/{licenseKey}")]
        public async Task<IActionResult> FreezeLicense(string licenseKey)
        {
            var userId = int.Parse(User.FindFirst("Id").Value);
            return await _licenseService.FreezeLicenseAsync(licenseKey, userId);
        }

        [HttpPost("unfreeze/{licenseKey}")]
        public async Task<IActionResult> UnfreezeLicense(string licenseKey)
        {
            var userId = int.Parse(User.FindFirst("Id").Value);
            return await _licenseService.UnfreezeLicenseAsync(licenseKey, userId);
        }

        [HttpDelete("delete/{licenseKey}")]
        public async Task<IActionResult> DeleteLicense(string licenseKey)
        {
            var userId = int.Parse(User.FindFirst("Id").Value);
            return await _licenseService.DeleteLicenseAsync(licenseKey, userId);
        }
    }

}
