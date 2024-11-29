namespace LicenseManagementAPI.Application.Interfaces
{
    public interface IEncryptionService
    {
        string Encrypt(string plainText, string key, string iv);
        string Decrypt(string cipherText, string key, string iv);
    }

}
