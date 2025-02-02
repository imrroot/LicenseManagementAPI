using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LicenseManagementLibrary.src.Models.DTOs;

namespace LicenseManagementLibrary.src.Services
{
    internal class EncryptionService
    {
        private readonly string _key;
        private readonly string _iv;

        public EncryptionService(string key, string iv)
        {
            _key = key;
            _iv = iv;
        }
        
        public string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(this._key.PadRight(32).Substring(0, 32)); // Ensure key is 32 bytes
            aes.IV = Encoding.UTF8.GetBytes(this._iv.PadRight(16).Substring(0, 16)); // Ensure IV is 16 bytes

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

        public CustomerResponseDto Decrypt(string cipherText)
        {
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(this._key.PadRight(32).Substring(0, 32)); // Ensure key is 32 bytes
            aes.IV = Encoding.UTF8.GetBytes(this._iv.PadRight(16).Substring(0, 16)); // Ensure IV is 16 bytes

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
            using (var cryptoStream = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            {
                using var reader = new StreamReader(cryptoStream);
                var decryptedText = reader.ReadToEnd();
                try
                {
                    // Deserialize the JSON into CustomerRequestBodyDto
                    return JsonSerializer.Deserialize<CustomerResponseDto>(decryptedText, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true // Handle case-insensitivity for property names
                    }) ?? throw new FormatException("Decrypted data is not in the expected JSON format.");
                }
                catch (JsonException ex)
                {
                    throw new FormatException("Error deserializing JSON: " + ex.Message);
                }
            }
        }
    }
}
