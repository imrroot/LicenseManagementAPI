using LicenseManagementAPI.Core.Entities;
using LicenseManagementAPI.Core.Enums;

namespace LicenseManagementAPI.Presentation.DTOs
{
    public class CustomerRequestBodyDto
    {
        public string License { get; set; }
        public string HWID { get; set; }
        public string IP { get; set; }

    }
    public class EncryptedRequestDto
    {
        public string AppKey { get; set; }
        public string EncryptedData { get; set; }
    }

    public class CustomerResponseDto
    {
        public string Message { get; set; }
        public CustomerLicenseDTO? License { get; set; }
    }

    public class CustomerLicenseDTO
    {
        public string? LicenseKey { get; set; }
        public LicenseStatus? Status { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? LastUsedDate { get; set; }
        public CustomerSubscriptionDTO? Subscription { get; set; }
    }

    public class CustomerSubscriptionDTO
    {
        public string Name { get; set; }
        public int AccessLevel { get; set; }
    }
    
}
