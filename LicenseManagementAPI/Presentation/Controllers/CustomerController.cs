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
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] EncryptedRequestDto request)
        {
            return  await _customerService.CustomerLoginAsync(request);
            
        }

        [HttpPost("status")]
        public async Task<IActionResult> CheckStatus([FromBody] EncryptedRequestDto request)
        {
            return  await _customerService.CustomerLicenseGetStatusAsync(request);
            
        }

        [HttpPost("renew")]
        public async Task<IActionResult> Renew([FromBody] EncryptedRequestDto request)
        {
            return await _customerService.CustomerRenewLicenseAsync(request);
        }

        [HttpPost("ban")]
        public async Task<IActionResult> Ban([FromBody] EncryptedRequestDto request)
        {
            return await _customerService.CustomerBanLicenseAsync(request);
        }
    }

}
