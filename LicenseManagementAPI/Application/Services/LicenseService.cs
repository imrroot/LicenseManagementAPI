using LicenseManagementAPI.Application.Interfaces;
using LicenseManagementAPI.Core.Entities;
using LicenseManagementAPI.Core.Enums;
using LicenseManagementAPI.Core.Interfaces;
using LicenseManagementAPI.Presentation.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace LicenseManagementAPI.Application.Services
{
    public class LicenseService : ILicenseService
    {
        private readonly ILicenseRepository _licenseRepository;
        private readonly IAppRepository _appRepository;
        private readonly IEncryptionService _encryptionService;
        public LicenseService(ILicenseRepository licenseRepository, IAppRepository appRepository, IEncryptionService encryptionService)
        {
            _licenseRepository = licenseRepository;
            _appRepository = appRepository;
            _encryptionService = encryptionService;
        }

        
        public async Task<IActionResult> FreezeLicenseAsync(string licenseKey,int userId)
        {
            var license = await _licenseRepository.GetLicenseByUserAsync(licenseKey,userId);
            if (license == null)
                return new NotFoundObjectResult( new { Message = "License doesn't exist." });

            if (license.Status == LicenseStatus.Frozen)
                return new BadRequestObjectResult( new {Message = "License is already frozen." });

            license.Status = LicenseStatus.Frozen;
            license.FreezeStartTime = DateTime.UtcNow;
            await _licenseRepository.UpdateLicenseAsync(license);

            return new OkObjectResult(new{Message = "License froze successfully."});
        }

        public async Task<IActionResult> UnfreezeLicenseAsync(string licenseKey,int userId)
        {
            var license = await _licenseRepository.GetLicenseByUserAsync(licenseKey,userId);
            if (license == null)
                return new NotFoundObjectResult( new{Message = "License doesn't exist." });

            if (license.Status != LicenseStatus.Frozen)
                return new BadRequestObjectResult( new {Message = "License is not froze."});

            license.FreezeEndTime = DateTime.UtcNow;
            license.AdjustExpiryDateAfterUnfreeze();
            license.Status = LicenseStatus.Active;
            await _licenseRepository.UpdateLicenseAsync(license);

            return new OkObjectResult( new{Message = "License Unfroze successfully."});
        }
        public async Task<IActionResult> CreateLicenseAsync(CreateLicenseDto createLicenseDto,int userId)
        {
            var app = await _appRepository.GetAppByIdWithSubscriptionsAsync(createLicenseDto.ApplicationId,userId);

            if (app == null) 
                return new NotFoundObjectResult(new {Message = "Application doesn't exist."});
            if (app.IsFrozen) 
                return new BadRequestObjectResult(new {Message = "Cannot create a license. The application is currently frozen."});

            var subscription = app.Subscriptions.FirstOrDefault(s => s.Name == createLicenseDto.Subscription);
            if (subscription == null) 
                return new NotFoundObjectResult(new {Message = "Subscription level not found for this application."} );
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
            license.SetExpiryDate();
            license.Pattern = license.GenerateCustomPattern();
            await _licenseRepository.AddLicenseAsync(license);
              return new OkObjectResult(new { Message = "License created successfully.", LicenseKey = license.Pattern, Expiredate = license.ExpiryDate });
        }

        public async Task<IActionResult> BanLicenseAsync(string licenseKey,int userId)
        {
            var license = await _licenseRepository.GetLicenseByUserAsync(licenseKey,userId);
            if (license == null)
                return new NotFoundObjectResult(new { Message = "License doesn't exist." });
            license.Status = LicenseStatus.Banned;
            await _licenseRepository.AddLicenseAsync(license);
            return new OkObjectResult(new {Message = "License banned."});
        }
        

        public async Task<IActionResult> GetLicenseStatusAsync(string licenseKey,int userId)
        {
            var license = await _licenseRepository.GetLicenseByUserAsync(licenseKey,userId);
            if (license == null) return new NotFoundObjectResult(new {Message = "License doesn't exist."});

            return new OkObjectResult( license.Status.ToString());
        }

        public async Task<IActionResult> DeleteLicenseAsync(string licenseKey, int userId)
        {
            var lincese = await _licenseRepository.GetLicenseByUserAsync(licenseKey,userId);
            if(lincese == null) return new NotFoundObjectResult(new {Message = "License doesn't exist."});
            await _licenseRepository.DeleteLicenseAsync(lincese);
            return new OkObjectResult(new {Message = "License is now deleted."});
            
        }

        public async Task<IActionResult> CustomerValidateLicense(string licenseKey, string hwid, string ip)
        {
            var license = await _licenseRepository.GetLicenseByKeyAsync(licenseKey);
            if (license == null)
                return new NotFoundObjectResult(new {Message = "License doesn't exist."});

            if (license.Status == LicenseStatus.Banned)
                return new BadRequestObjectResult(new {Message = "License banned."});
            

            if (license.ExpiryDate < DateTime.UtcNow)
                return new BadRequestObjectResult(new {Message = "License expired."});

            if (!string.IsNullOrEmpty(license.HWID) && license.HWID != hwid)
                return new BadRequestObjectResult(new {Message = "HWID mismatch"});

            if (!string.IsNullOrEmpty(license.IP) && license.IP != ip)
                return new BadRequestObjectResult(new {Message = "IP mismatch"});

            return new OkObjectResult(new {Message = "License is valid."});
        }
        public async Task<IActionResult> CustomerLoginAsync(EncryptedRequestDto encryptedRequest)
        {
            var app = await _appRepository.GetAppByKeyAsync(encryptedRequest.AppKey);
            if (app == null)
                return new NotFoundObjectResult(new {Message = "Application doesn't exist."});
            var decryptedData = _encryptionService.Decrypt(encryptedRequest.EncryptedData, app.Secret, app.User.OwnerId);
            var data = decryptedData.Split(',');
            var (licenseKey, hwid, ip) = (data[0], data[1], data[2]);
            var license = await _licenseRepository.GetLicenseByKeyAsync(licenseKey);
            if (license == null) return new NotFoundObjectResult(new {Message = "License doesn't exist."});

            if (!string.IsNullOrEmpty(license.HWID) && license.HWID != hwid)
                return new BadRequestObjectResult(new {Message = "HWID mismatch"});
            if (!string.IsNullOrEmpty(license.IP) && license.IP != ip)
                return new BadRequestObjectResult(new {Message = "IP mismatch"});

            license.HWID = hwid;
            license.IP = ip;
            license.Status = LicenseStatus.Active;
            license.LastUsedDate = DateTime.UtcNow;

            await _licenseRepository.UpdateLicenseAsync(license);
            return new OkObjectResult(new {Message = "Login was successful."});
        }

        public async Task<IActionResult> GetCustomerLicenseStatusAsync(EncryptedRequestDto encryptedRequest)
        {
            var app = await _appRepository.GetAppByKeyAsync(encryptedRequest.AppKey);
            if (app == null)
                return new NotFoundObjectResult(new {Message = "Application doesn't exist."});
            var decryptedData = _encryptionService.Decrypt(encryptedRequest.EncryptedData,app.Secret,app.User.OwnerId);
            var licenseKey = decryptedData.Split(',')[0];
            
            var license = await _licenseRepository.GetLicenseByKeyAsync(licenseKey);
            if (license == null) return new NotFoundObjectResult(new {Message = "License doesn't exist."});

            return new OkObjectResult( $"Status: {license.Status}, Expiry: {license.ExpiryDate}");
        }
        public async Task<IActionResult> CustomerRenewLicenseAsync(EncryptedRequestDto encryptedRequest)
        {
            var app = await _appRepository.GetAppByKeyAsync(encryptedRequest.AppKey);
            if (app == null)
                return new NotFoundObjectResult(new {Message = "Application doesn't exist."});
            var decryptedData = _encryptionService.Decrypt(encryptedRequest.EncryptedData, app.Secret, app.User.OwnerId);
            var (licenseKey, hwid, ip) = (
                decryptedData.Split(',')[0],
                decryptedData.Split(',')[1],
                decryptedData.Split(',')[2]
            );
             await CustomerValidateLicense(licenseKey, hwid, ip);
            var license = await _licenseRepository.GetLicenseByKeyAsync(licenseKey);
            if (license == null)
                return new NotFoundObjectResult(new {Message = "License doesn't exist."});

            if (license.Status == LicenseStatus.Banned)
                return new BadRequestObjectResult(new {Message = "Cannot renew a banned license"});

            if (license.ExpiryDate >= DateTime.UtcNow && (license.ExpiryDate - DateTime.UtcNow).TotalDays > 7)
                return new BadRequestObjectResult(new {Message = "License is not close enough to expiry for renewal"});
            
            license.SetExpiryDate();
            await _licenseRepository.UpdateLicenseAsync(license);

            return new OkObjectResult( new {Message = $"License renewed successfully. New expiry date: {license.ExpiryDate}"});
        }

        public async Task<IActionResult> CustomerBanLicenseAsync(EncryptedRequestDto encryptedRequest)
        {
            var app = await _appRepository.GetAppByKeyAsync(encryptedRequest.AppKey);
            if (app == null)
                return new NotFoundObjectResult(new {Message = "Application doesn't exist."});
            var decryptedData = _encryptionService.Decrypt(encryptedRequest.EncryptedData, app.Secret, app.User.OwnerId);
            var (licenseKey, hwid, ip) = (
                decryptedData.Split(',')[0],
                decryptedData.Split(',')[1],
                decryptedData.Split(',')[2]
            );
            var license = await _licenseRepository.GetLicenseByKeyAsync(licenseKey);
            await CustomerValidateLicense(licenseKey, hwid, ip);
            if (license.HWID != hwid || license.IP != ip)
                return new BadRequestObjectResult(new {Message = "HWID or IP does not match the registered device"});
            license.Status = LicenseStatus.Banned;
            await _licenseRepository.UpdateLicenseAsync(license);
            return new OkObjectResult(new {Message = "License banned successfully."});
            
        }
    }

}
