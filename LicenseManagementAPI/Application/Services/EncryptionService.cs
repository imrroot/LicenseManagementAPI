﻿namespace LicenseManagementAPI.Application.Services
{
    using LicenseManagementAPI.Application.Interfaces;
    using System.Security.Cryptography;
    using System.Text;

    public class EncryptionService : IEncryptionService
    {
        public string Encrypt(string plainText, string key, string iv)
        {
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32)); // Ensure key is 32 bytes
            aes.IV = Encoding.UTF8.GetBytes(iv.PadRight(16).Substring(0, 16)); // Ensure IV is 16 bytes

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            using (var cryptoStream = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                using (var writer = new StreamWriter(cryptoStream))
                {
                    writer.Write(plainText);
                }
            }
            return Convert.ToBase64String(ms.ToArray());
        }

        public string Decrypt(string cipherText, string key, string iv)
        {
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32)); // Ensure key is 32 bytes
            aes.IV = Encoding.UTF8.GetBytes(iv.PadRight(16).Substring(0, 16)); // Ensure IV is 16 bytes

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
            using (var cryptoStream = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            {
                using var reader = new StreamReader(cryptoStream);
                return reader.ReadToEnd();
            }
        }
    }

}
