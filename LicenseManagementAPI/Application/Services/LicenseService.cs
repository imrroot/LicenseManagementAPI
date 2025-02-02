using LicenseManagementAPI.Application.Interfaces;
using LicenseManagementAPI.Core.Entities;
using LicenseManagementAPI.Core.Enums;
using LicenseManagementAPI.Presentation.DTOs;
using LicenseManagementAPI.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LicenseManagementAPI.Application.Services
{
    public class LicenseService : ILicenseService
    {

        private readonly ILicenseRepository _licenseRepository;
        private readonly IAppRepository _appRepository;

        public LicenseService(ILicenseRepository licenseRepository, IAppRepository appRepository)
        {
            _licenseRepository = licenseRepository;
            _appRepository = appRepository;
        }


        public async Task<IActionResult> GetLicensesAppAsync(int appId, int userId, int pageNumber, int pageSize)
        {
            var app = await _appRepository.GetAppByIdWithLicensesAsync(appId, userId);
            if (app == null) return new NotFoundObjectResult(new { Message = "Application doesn't exist." });
            if (app.Licenses == null || app.Licenses.Count == 0)
                return new NotFoundObjectResult(new { Message = "No licenses found for this application." });

            var totalLicenses = app.Licenses.Count;
            var licenses = app.Licenses
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(l => new LicenseResponeDto
                {
                    LicenseKey = l.Pattern,
                    Note = l.Note,
                    Status = l.Status,
                    ExpirationDate = l.ExpiryDate,
                    LastUsedDate = l.LastUsedDate,
                    CreatedDate = l.CreationDate,
                    FreezeStartTime = l.FreezeStartTime,
                    FreezeEndTime = l.FreezeEndTime,
                    HWID = l.HWID,
                    IP = l.IP,
                })
                .ToList();

            var pagedResponse = new LicensePagedResponseDto<LicenseResponeDto>
            {
                TotalCount = totalLicenses,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Licenses = licenses
            };

            return new OkObjectResult(pagedResponse);
        }


        public async Task<IActionResult> FreezeLicenseAsync(string licenseKey, int userId)
        {
            var license = await _licenseRepository.GetLicenseByUserAsync(licenseKey, userId);
            if (license == null)
                return new NotFoundObjectResult(new { Message = "License doesn't exist." });

            if (license.Status == LicenseStatus.Frozen)
                return new BadRequestObjectResult(new { Message = "The license is already frozen." });

            license.Status = LicenseStatus.Frozen;
            license.FreezeStartTime = DateTime.UtcNow;
            await _licenseRepository.UpdateLicenseAsync(license);

            return new OkObjectResult(new { Message = "The license was frozen successfully." });
        }

        public async Task<IActionResult> UnfreezeLicenseAsync(string licenseKey, int userId)
        {
            var license = await _licenseRepository.GetLicenseByUserAsync(licenseKey, userId);
            if (license == null)
                return new NotFoundObjectResult(new { Message = "License doesn't exist." });

            if (license.Status != LicenseStatus.Frozen)
                return new BadRequestObjectResult(new { Message = "The license is not frozen." });

            license.FreezeEndTime = DateTime.UtcNow;
            license.AdjustExpiryDateAfterUnfreeze();
            license.Status = LicenseStatus.NotUsed;
            await _licenseRepository.UpdateLicenseAsync(license);

            return new OkObjectResult(new { Message = "The license was unfrozen successfully." });
        }

        public async Task<IActionResult> CreateLicenseAsync(CreateLicenseDto createLicenseDto, int userId)
        {
            var app = await _appRepository.GetAppByIdWithSubscriptionsAsync(createLicenseDto.ApplicationId, userId);

            if (app == null)
                return new NotFoundObjectResult(new { Message = "Application doesn't exist." });
            if (app.IsFrozen)
                return new BadRequestObjectResult(new
                    { Message = "Cannot create a license. The application is currently frozen." });

            var subscription = app.Subscriptions.FirstOrDefault(s => s.Name == createLicenseDto.Subscription);
            if (subscription == null)
                return new NotFoundObjectResult(new { Message = "Subscription level not found for this application." });

            var licenses = new List<License>();

            for (int i = 0; i < createLicenseDto.Amount; i++)
            {
                var license = new License
                {
                    ApplicationId = app.Id,
                    SubscriptionId = subscription.Id,
                    Pattern = createLicenseDto.Pattern ?? "****-****-****-****",
                    CreationDate = DateTime.UtcNow,
                    Status = LicenseStatus.NotUsed,
                    Duration = createLicenseDto.Duration,
                    ExpiryUnit = createLicenseDto.ExpiryUnit,
                    HWID = createLicenseDto.InitialHWID,
                    IP = createLicenseDto.InitialIP,
                    Note = createLicenseDto.Notes ?? string.Empty
                };
                license.AdjustExpiryDate();
                license.Pattern = license.GenerateCustomPattern();
                licenses.Add(license);
            }

            await _licenseRepository.AddLicenseAsync(licenses); 
            return new OkObjectResult(new
            {
                Message = $"{licenses.Count} licenses created successfully.",
                Licenses = licenses.Select(l => new
                {
                    LicenseKey = l.Pattern,
                    ExpireDate = l.ExpiryDate
                })
            });
        }

        public async Task<IActionResult> BanLicenseAsync(string licenseKey, int userId)
        {
            var license = await _licenseRepository.GetLicenseByUserAsync(licenseKey, userId);
            if (license == null)
                return new NotFoundObjectResult(new { Message = "License doesn't exist." });
            if (license.Status == LicenseStatus.Banned)
                return new BadRequestObjectResult(new { Message = "License is already banned !" });
            license.Status = LicenseStatus.Banned;
            await _licenseRepository.UpdateLicenseAsync(license);
            return new OkObjectResult(new { Message = $"License {licenseKey} was banned successfully." });
        }
        public async Task<IActionResult> UnBanLicenseAsync(string licenseKey, int userId)
        {
            var license = await _licenseRepository.GetLicenseByUserAsync(licenseKey, userId);
            if (license == null)
                return new NotFoundObjectResult(new { Message = "License doesn't exist." });
            license.Status = LicenseStatus.NotUsed;
            await _licenseRepository.UpdateLicenseAsync(license);
            return new OkObjectResult(new { Message = $"License {licenseKey} was unbanned successfully." });
        }


        public async Task<IActionResult> GetLicenseStatusAsync(string licenseKey, int userId)
        {
            var license = await _licenseRepository.GetLicenseByUserAsync(licenseKey, userId);
            if (license == null) return new NotFoundObjectResult(new { Message = "License doesn't exist." });

            return new OkObjectResult(license.Status.ToString());
        }

        public async Task<IActionResult> DeleteLicenseAsync(string licenseKey, int userId)
        {
            var lincese = await _licenseRepository.GetLicenseByUserAsync(licenseKey, userId);
            if (lincese == null) return new NotFoundObjectResult(new { Message = "License doesn't exist." });
            await _licenseRepository.DeleteLicenseAsync(lincese);
            return new OkObjectResult(new { Message = $"License {licenseKey} was deleted successfully." });
        }

        public async Task<IActionResult> DeleteAllAsync(int appId, int userId, LicenseFilterType filterType = LicenseFilterType.All)
        {
            var app = await _appRepository.GetAppByIdWithLicensesAsync(appId, userId);
            if (app == null) return new NotFoundObjectResult(new { Message = "App doesn't exist." });
            if (app.Licenses == null || app.Licenses.Count == 0) 
                return new NotFoundObjectResult(new { Message = "No licenses exist for this application." });

            // the predicate based on filterType
            Func<License, bool> predicate = filterType switch
            {
                LicenseFilterType.All => _ => true, 
                LicenseFilterType.Banned => l => l.Status == LicenseStatus.Banned, 
                LicenseFilterType.Unused => l => l.Status == LicenseStatus.NotUsed, 
                LicenseFilterType.Expired => l => l.ExpiryDate < DateTime.UtcNow, 
                _ => null
            };

            await _licenseRepository.DeleteAllAsync(predicate);
            return new OkObjectResult(new { Message = $"Licenses with {filterType.ToString()} status been deleted successfully." });
        }

        public async Task<IActionResult> DeleteDeviceAsync(string licenseKey, int userId)
        {
            var license = await _licenseRepository.GetLicenseByUserAsync(licenseKey, userId);
            if (license == null) return new NotFoundObjectResult(new { Message = "License doesn't exist." });
            license.HWID = "";
            license.IP = "";
            await _licenseRepository.UpdateLicenseAsync(license);
            return new OkObjectResult(new { Message = $"Hwid and IP removed from lincese {license.Pattern} successfully."  });
        }
    }
}