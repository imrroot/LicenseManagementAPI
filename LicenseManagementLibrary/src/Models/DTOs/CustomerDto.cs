using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseManagementLibrary.src.Models.DTOs
{
    public class CustomerRequestBodyDto
    {
        public string License { get; set; }
        public string HWID { get; set; }
        public string IP { get; set; }

    }
    public class EncryptedRequestDto
    {
        public string appKey { get; set; }
        public string encryptedData { get; set; }
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

    public enum LicenseStatus
    {
        Active,
        Frozen,
        Banned,
        Expired,
        NotUsed
    }

    public class CustomerSubscriptionDTO
    {
        public string Name { get; set; }
        public int AccessLevel { get; set; }
    }
}
