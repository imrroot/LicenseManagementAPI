using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LicenseManagementLibrary.src.Models.DTOs;

namespace LicenseManagementLibrary.src.Services
{
    internal class RequestService
    {
        private readonly HttpClient _client;
        private readonly string _apiUrl;
        private readonly EncryptionService _encryptionService;
        public RequestService(string apiUrl, EncryptionService encryptionService)
        {
            _client = new HttpClient();
            _apiUrl = apiUrl;
            _encryptionService = encryptionService;
        }

        public async Task<CustomerResponseDto> SendRequest(string endpoint, string encryptedData)
        {
            CustomerResponseDto responseDto = null;
            try
            {
                var content = new StringContent(encryptedData, Encoding.UTF8, "application/json");
                var response = await _client.PostAsync($"{_apiUrl}/{endpoint}", content);
                var rescontent = await response.Content.ReadAsStringAsync();
                responseDto = _encryptionService.Decrypt(rescontent);

            }
            catch (Exception ex)
            {

            }

            return responseDto;
        }
    }
}
