using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;

namespace LicenseManagementAPI.Core.Entities
{
    public partial class App
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Secret { get; set; } = GenerateSecret();
        public string AppKey { get; set; }
        public bool IsFrozen { get; set; }
        public int UserId { get; set; } //Foreign Key to User
        public User User { get; set; }
        public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
        public ICollection<License> Licenses { get; set; } = new List<License>();
    }
    public partial class App
    {
        //Generate Unique Secret as Key of Requests Encryption
        private static string GenerateSecret()
        {
            using var sha256 = SHA256.Create();
            var secretBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));
            return BitConverter.ToString(secretBytes).Replace("-", "").ToLower();
        }
        //Generate Unique AppKey for Find App
        public void GenerateAppKey(string ownerId)
        {
            using var sha256 = SHA256.Create();
            var combined = $"{ownerId}{Secret}";
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
            AppKey = Convert.ToBase64String(hash)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "")
                .Substring(0, 10);
        }
    }
}
