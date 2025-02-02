using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LicenseManagementLibrary.src.Models;
using LicenseManagementLibrary.src.Models.DTOs;
using LicenseManagementLibrary.src.Services;

namespace LicenseManagementLibrary.src.Core
{
    public class LicenseManager
    {
        private readonly EncryptionService _encryptionService;
        private readonly RequestService _requestService;
        private string _licenseKey;
        private string _appkey;
        public License License { get; private set; }
        public Subscription Subscription { get; private set; }
        public LicenseManager(string baseUrl, string appKey, string secret, string ownerId)
        {
            _encryptionService = new EncryptionService(secret, ownerId);
            _requestService = new RequestService(baseUrl,_encryptionService);
            _appkey = appKey;
        }
        public async Task<string> InitializeAsync(string licenseKey)
        {
            _licenseKey = licenseKey;
            var hwid = SystemService.GetHWID();
            //var hwid = "G4G1xg2G15";
            var ip = SystemService.GetIP();

            var requestData = new CustomerRequestBodyDto { License = licenseKey, HWID = hwid, IP = ip };

            var encryptedData = _encryptionService.Encrypt(await JsonService.SerializeDto(requestData));
            var json = new EncryptedRequestDto() { appKey = _appkey, encryptedData = encryptedData };
            var response = await _requestService.SendRequest("customer/login", await JsonService.SerializeDto(json));

            if (response.License is not null)
            {
                License = new License(response.License);
                Subscription = new Subscription(response.License.Subscription);
            }
            return response.Message;
        }
        public async Task<string> GetStatus(string licenseKey)
        {
            _licenseKey = licenseKey;
            var hwid = SystemService.GetHWID();
            var ip = SystemService.GetIP();

            var requestData = new CustomerRequestBodyDto { License = licenseKey, HWID = hwid, IP = ip };

            var encryptedData = _encryptionService.Encrypt(await JsonService.SerializeDto(requestData));
            var json = new EncryptedRequestDto() { appKey = _appkey, encryptedData = encryptedData };
            var response = await _requestService.SendRequest("customer/status", await JsonService.SerializeDto(json));

            if (response.License is not null)
            {
                License = new License(response.License);
                Subscription = new Subscription(response.License.Subscription);
            }
            return response.Message;
        }
        public async Task<string> Ban(string licenseKey)
        {
            _licenseKey = licenseKey;
            var hwid = SystemService.GetHWID();
            var ip = SystemService.GetIP();

            var requestData = new CustomerRequestBodyDto { License = licenseKey, HWID = hwid, IP = ip };

            var encryptedData = _encryptionService.Encrypt(await JsonService.SerializeDto(requestData));
            var json = new EncryptedRequestDto() { appKey = _appkey, encryptedData = encryptedData };
            var response = await _requestService.SendRequest("customer/ban", await JsonService.SerializeDto(json));

            return response.Message;
        }
        public bool HasAccess(int requiredLevel)
        {
            return Subscription != null && Subscription.AccessLevel <= requiredLevel;
        }
    }
}
