
using LicenseManagementAPI.Presentation.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace LicenseManagementAPI.Application.Interfaces
{
    public interface IAppService
    {
        Task<IActionResult> CreateAppAsync(CreateApplicationDto CreateappDto, int userId);
        Task<IActionResult> DeleteAppAsync(int appId, int userId);
        Task<IActionResult>FreezeAppAsync(int appId, int userId);
        Task<IActionResult>UnfreezeAppAsync(int appId, int userId);
        Task<IEnumerable<ApplicationResponseDto>> GetUserAppsAsync(int userId);
    }

}
