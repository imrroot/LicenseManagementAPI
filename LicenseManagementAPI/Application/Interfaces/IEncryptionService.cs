using LicenseManagementAPI.Presentation.DTOs;

namespace LicenseManagementAPI.Application.Interfaces
{
    public interface IEncryptionService
    {
        string Encrypt(string plainText, string key, string iv);
        CustomerRequestBodyDto Decrypt(string cipherText, string key, string iv);
    }

}
