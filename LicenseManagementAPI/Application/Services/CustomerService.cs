using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using LicenseManagementAPI.Application.Interfaces;
using LicenseManagementAPI.Core.Entities;
using LicenseManagementAPI.Core.Enums;
using LicenseManagementAPI.Infrastructure.Interfaces;
using LicenseManagementAPI.Presentation.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace LicenseManagementAPI.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ILicenseRepository _licenseRepository;
        private readonly IAppRepository _appRepository;
        private readonly IEncryptionService _encryptionService;


        public CustomerService(ILicenseRepository licenseRepository, IAppRepository appRepository,
            IEncryptionService encryptionService)
        {
            _licenseRepository = licenseRepository;
            _appRepository = appRepository;
            _encryptionService = encryptionService;
        }

        private string EncryptedResponse(object data, string secret, string ownerId)
        {
            var json = JsonSerializer.Serialize(data);
            return  _encryptionService.Encrypt(json, secret, ownerId);
        }

        public async Task<IActionResult> CustomerValidateLicense(string licenseKey, string hwid, string ip, App app)
        {
            var license = await _licenseRepository.GetLicenseByKeyAsync(licenseKey);
            if (license == null)
                return new NotFoundObjectResult(EncryptedResponse(new CustomerResponseDto { Message = "License doesn't exist." }, app.Secret, app.User.OwnerId));

            if (license.Status == LicenseStatus.Banned)
                return new BadRequestObjectResult(EncryptedResponse(new CustomerResponseDto { Message = "Unfortunately license is banned can't use it anymore." }, app.Secret, app.User.OwnerId));

            if (license.ExpiryDate < DateTime.UtcNow)
                return new BadRequestObjectResult(EncryptedResponse(new CustomerResponseDto { Message = "Unfortunately license is expired can't use it anymore." }, app.Secret, app.User.OwnerId));

            if (!string.IsNullOrEmpty(license.HWID) && license.HWID != hwid)
                return new BadRequestObjectResult(EncryptedResponse(new CustomerResponseDto { Message = "Device does not match the registered device !" }, app.Secret, app.User.OwnerId));

            if (!string.IsNullOrEmpty(license.IP) && license.IP != ip)
                return new BadRequestObjectResult(EncryptedResponse(new CustomerResponseDto { Message = "IP does not match the registered info !" }, app.Secret, app.User.OwnerId));

            return new OkObjectResult(EncryptedResponse(new CustomerResponseDto { Message = "License is valid." }, app.Secret, app.User.OwnerId));
        }

        public async Task<IActionResult> CustomerLoginAsync(EncryptedRequestDto encryptedRequest)
        {
            var app = await _appRepository.GetAppByKeyAsync(encryptedRequest.AppKey);
            if (app == null)
                return new NotFoundObjectResult(EncryptedResponse(new CustomerResponseDto { Message = "Application doesn't exist." }, app.Secret, app.User.OwnerId));
            var body =
                _encryptionService.Decrypt(encryptedRequest.EncryptedData, app.Secret, app.User.OwnerId);

            var validationResult = await CustomerValidateLicense(body.License, body.HWID, body.IP,app);
            if (validationResult is ObjectResult objectResult && objectResult.StatusCode != 200)
                return validationResult;

            var license = await _licenseRepository.GetLicenseByKeyAsync(body.License);

            license.HWID = body.HWID;
            license.IP = body.IP;
            license.Status = LicenseStatus.Active;
            license.LastUsedDate = DateTime.UtcNow;

            var json = new CustomerResponseDto
            {
                Message = "Login was successful.",
                License = new CustomerLicenseDTO()
                {
                    LicenseKey = license.Pattern,
                    Status = license.Status,
                    ExpirationDate = license.ExpiryDate,
                    LastUsedDate = license.LastUsedDate,
                    Subscription = new CustomerSubscriptionDTO()
                    {
                        Name = license.Subscription.Name,
                        AccessLevel = license.Subscription.Level
                    },
                }
            };
            await _licenseRepository.UpdateLicenseAsync(license);

            return new OkObjectResult(EncryptedResponse(json, app.Secret, app.User.OwnerId));
        }

        public async Task<IActionResult> CustomerLicenseGetStatusAsync(EncryptedRequestDto encryptedRequest)
        {
            var app = await _appRepository.GetAppByKeyAsync(encryptedRequest.AppKey);
            if (app == null)
                return new NotFoundObjectResult(EncryptedResponse(new CustomerResponseDto { Message = "Application doesn't exist." }, app.Secret, app.User.OwnerId));
            var body =
                _encryptionService.Decrypt(encryptedRequest.EncryptedData, app.Secret, app.User.OwnerId);


            var validationResult = await CustomerValidateLicense(body.License, body.HWID, body.IP, app);
            if (validationResult is ObjectResult objectResult && objectResult.StatusCode != 200)
                return validationResult;


            var license = await _licenseRepository.GetLicenseByKeyAsync(body.License);
            var json = new CustomerResponseDto
            {
                Message = "Login was successful.",
                License = new CustomerLicenseDTO()
                {
                    LicenseKey = license.Pattern,
                    Status = license.Status,
                    ExpirationDate = license.ExpiryDate,
                    LastUsedDate = license.LastUsedDate,
                    Subscription = new CustomerSubscriptionDTO()
                    {
                        Name = license.Subscription.Name,
                        AccessLevel = license.Subscription.Level
                    },
                }
            };
            
            return new OkObjectResult(EncryptedResponse(json, app.Secret, app.User.OwnerId));
        }

        public async Task<IActionResult> CustomerRenewLicenseAsync(EncryptedRequestDto encryptedRequest)
        {
            var app = await _appRepository.GetAppByKeyAsync(encryptedRequest.AppKey);
            if (app == null)
                return new NotFoundObjectResult(EncryptedResponse(new CustomerResponseDto { Message = "Application doesn't exist." }, app.Secret, app.User.OwnerId));

            var body =
                _encryptionService.Decrypt(encryptedRequest.EncryptedData, app.Secret, app.User.OwnerId);


            var license = await _licenseRepository.GetLicenseByKeyAsync(body.License);
            if (license == null)
                return new NotFoundObjectResult(EncryptedResponse(new CustomerResponseDto { Message = "License doesn't exist." }, app.Secret, app.User.OwnerId));

            if (license.Status == LicenseStatus.Banned)
                return new BadRequestObjectResult(EncryptedResponse(new CustomerResponseDto { Message = "Cannot renew a banned license" }, app.Secret, app.User.OwnerId));

            if (license.ExpiryDate >= DateTime.UtcNow && (license.ExpiryDate - DateTime.UtcNow).TotalDays > 7)
                return new BadRequestObjectResult(EncryptedResponse(new CustomerResponseDto { Message = "License is not close enough to expiry for renewal" }, app.Secret, app.User.OwnerId));

            license.AdjustExpiryDate();
            await _licenseRepository.UpdateLicenseAsync(license);
            var json = new CustomerResponseDto
            {
                Message = "License renewed successfully",
                License = new CustomerLicenseDTO()
                {
                    ExpirationDate = license.ExpiryDate,

                }
            };
            
            return new OkObjectResult(EncryptedResponse(json, app.Secret, app.User.OwnerId));
        }

        public async Task<IActionResult> CustomerBanLicenseAsync(EncryptedRequestDto encryptedRequest)
        {
            var app = await _appRepository.GetAppByKeyAsync(encryptedRequest.AppKey);
            if (app == null)
                return new NotFoundObjectResult(EncryptedResponse(new CustomerResponseDto { Message = "Application doesn't exist." }, app.Secret, app.User.OwnerId));
            var body =
                _encryptionService.Decrypt(encryptedRequest.EncryptedData, app.Secret, app.User.OwnerId);
            var license = await _licenseRepository.GetLicenseByKeyAsync(body.License);

            if (license == null)
                return new NotFoundObjectResult(EncryptedResponse(new CustomerResponseDto{Message = "License doesn't exist." }, app.Secret, app.User.OwnerId));

            license.Status = LicenseStatus.Banned;
            await _licenseRepository.UpdateLicenseAsync(license);
            var json = new CustomerResponseDto
            {
                Message = "License banned successfully."
            };
            
            return new OkObjectResult(EncryptedResponse(json, app.Secret, app.User.OwnerId));
        }
    }
}
