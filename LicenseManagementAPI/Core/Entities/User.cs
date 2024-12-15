using System.Security.Cryptography;

namespace LicenseManagementAPI.Core.Entities
{
    public partial class User
    {
        public int UserId { get; set; }
        public string OwnerId { get; set; } = GenerateShortId();
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        public ICollection<App> Apps { get; set; } = new List<App>();

    }
    public partial class User
    {
        //Generate Unique Secret as IV (16 Byte) of Requests Encryption 
        private static string GenerateShortId()
        {
            using var rng = new RNGCryptoServiceProvider();
            var data = new byte[12]; 
            rng.GetBytes(data);
            return Convert.ToBase64String(data)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "").Substring(0, 16); 
        }
    }
}
