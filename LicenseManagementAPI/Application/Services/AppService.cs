using LicenseManagementAPI.Application.Interfaces;
using LicenseManagementAPI.Core.Entities;
using LicenseManagementAPI.Core.Enums;
using LicenseManagementAPI.Infrastructure.Interfaces;
using LicenseManagementAPI.Presentation.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace LicenseManagementAPI.Application.Services
{
    public class AppService : IAppService
    {
        private readonly IAppRepository _appRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        public AppService(IAppRepository appRepository,IUserRepository userRepository,ISubscriptionRepository subscriptionRepository)
        {
            _appRepository = appRepository;
            _userRepository = userRepository;
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task<IActionResult> CreateAppAsync(CreateApplicationDto CreateappDto, int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            var app = new App
            {
                Name = CreateappDto.Name,
                Secret = Guid.NewGuid().ToString("N"),
                UserId = userId
            };
            app.GenerateAppKey(user.OwnerId);
            await _appRepository.AddAppAsync(app);
            return new OkObjectResult(new{Message = "Application created successfully"});
        }

        public async Task<IActionResult> DeleteAppAsync(int appId, int userId)
        {
            var app = await _appRepository.GetAppByIdAsync(appId, userId);
            if(app == null) return new NotFoundObjectResult(new{Message = "App not found."});
            await _appRepository.DeleteAppAsync(app);
            return new OkObjectResult(new{Message = "App deleted successfully."});
        }

        public async Task<IActionResult> FreezeAppAsync(int appId, int userId)
        {
            var app = await _appRepository.GetAppByIdAsync(appId, userId);
            if(app == null) return new NotFoundObjectResult(new{Message = "App not found."});
            if (app.IsFrozen)
                return new BadRequestObjectResult(new { Message = "Application is already frozen" });
            app.IsFrozen = true;
            foreach (var license in app.Licenses.Where(l => l.Status != LicenseStatus.Frozen))
            {
                license.Status = LicenseStatus.Frozen;
                license.FreezeStartTime = DateTime.UtcNow;
            }
            await _appRepository.UpdateAppAsync(app);
            return new OkObjectResult(new { Message = $"{app.Licenses.Count} licenses froze and application marked as frozen"});
        }
       
        public async Task<IActionResult> UnfreezeAppAsync(int appId, int userId)
        {
            var app = await _appRepository.GetAppByIdAsync(appId, userId);
            if(app == null) return new NotFoundObjectResult(new{Message = "App not found."});
            if (!app.IsFrozen)
                return new BadRequestObjectResult(new { Message = "Application is not frozen" });
            app.IsFrozen = false;
            foreach (var license in app.Licenses.Where(l => l.Status == LicenseStatus.Frozen && l.FreezeStartTime.HasValue))
            {
                license.FreezeEndTime = DateTime.UtcNow;
                license.AdjustExpiryDateAfterUnfreeze();
                license.Status = LicenseStatus.Active;
            }
            await _appRepository.UpdateAppAsync(app);
            return new OkObjectResult(new{Message = $"{app.Licenses.Count} licenses unfroze and application marked as active"});
        }

        public async Task<IActionResult> GetUserAppsAsync(int userId)
        {
            var apps = await _appRepository.GetAppsByUserIdAsync(userId);

            var response = new List<ApplicationWithSubscriptionsResponseDto>();

            foreach (var app in apps)
            {
                var subscriptions = await _subscriptionRepository.GetSubscriptionsByAppIdAsync(app.Id);

                var appDto = new ApplicationWithSubscriptionsResponseDto
                {
                    Id = app.Id,
                    Name = app.Name,
                    AppKey = app.AppKey,
                    AppSecret = app.Secret,
                    TotalLicenses = app.Licenses.Count(),
                    ActiveLicenses = app.Licenses.Count(l => l.Status == LicenseStatus.Active),
                    IsFrozen = app.IsFrozen,
                    Subscriptions = subscriptions.Select(s => new SubscriptionDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        AccessLevel = s.Level
                    }).ToList()
                };

                response.Add(appDto);
            }

            return new OkObjectResult(new
            {
                Applications = response
            });
        }
        
    }

}
