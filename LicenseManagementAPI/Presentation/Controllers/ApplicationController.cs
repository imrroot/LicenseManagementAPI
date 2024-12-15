using LicenseManagementAPI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using LicenseManagementAPI.Presentation.DTOs;
using LicenseManagementAPI.Core.Entities;

namespace LicenseManagementAPI.Presentation.Controllers
{
    [Authorize]
    [Route("api/app")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private readonly IAppService _appService;

        public ApplicationController(IAppService appService)
        {
            _appService = appService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateApplication([FromBody] CreateApplicationDto createApplicationDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var userId = int.Parse(User.FindFirst("Id").Value);
            return await _appService.CreateAppAsync(createApplicationDto, userId);
             
        }

        [HttpGet("list")]
        public async Task<IActionResult> ListApplications()
        {
            var userId = int.Parse(User.FindFirst("Id").Value);
            
            return await _appService.GetUserAppsAsync(userId);

            
        }
        

        [HttpPost("freeze/{id}")]
        public async Task<IActionResult> FreezeApplication(int id)
        {
            var userId = int.Parse(User.FindFirst("Id").Value);
            return await _appService.FreezeAppAsync(id, userId);
            
        }

        [HttpPost("unfreeze/{id}")]
        public async Task<IActionResult> UnfreezeApplication(int id)
        {
            var userId = int.Parse(User.FindFirst("Id").Value);
            return await _appService.UnfreezeAppAsync(id, userId);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteApplication(int id)
        { 
           var userId = int.Parse(User.FindFirst("Id").Value);
           return await _appService.DeleteAppAsync(id, userId);
           
        }
    }

}
