using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LicenseManagementLibrary.src.Models.DTOs;

namespace LicenseManagementLibrary.src.Models
{
    public class License
    {
        public string? LicenseKey { get; }
        public LicenseStatus? Status { get; }
        public DateTime? ExpirationDate { get; }
        public DateTime? LastUsedDate { get; }
        public License(CustomerLicenseDTO? license)
        {
            LicenseKey = license.LicenseKey;
            Status = license.Status;
            ExpirationDate = license.ExpirationDate;
            LastUsedDate = license.LastUsedDate;
        }
    }
}
