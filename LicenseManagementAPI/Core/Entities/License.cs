using LicenseManagementAPI.Core.Enums;

namespace LicenseManagementAPI.Core.Entities
{
    public partial class License
    {

        public int LicenseId { get; set; }

        public int ApplicationId { get; set; } // Foreign key to Application
        public App App { get; set; }

        private string _pattern = "****-****-****-****"; // Default pattern

        public string Pattern
        {
            get => _pattern;
            set => _pattern = !string.IsNullOrEmpty(value) ? value : _pattern;
        } 
        public string Note { get; set; } // Optional note for license
        public LicenseStatus Status { get; set; } = LicenseStatus.NotUsed;


        
        public ExpiryUnit ExpiryUnit { get; set; } = ExpiryUnit.Hours; // Expiry unit, default to hours
        public int Duration { get; set; } = 1; // Duration based on expiry unit
        public DateTime CreationDate { get; set; }
        public DateTime? LastUsedDate { get; set; }
        public DateTime ExpiryDate { get; set; }

        // New properties to track freeze times
        public DateTime? FreezeStartTime { get; set; }
        public DateTime? FreezeEndTime { get; set; }
       
        public int SubscriptionId { get; set; } // Foreign key to Subscription
        public Subscription Subscription { get; set; }
        public string HWID { get; set; } //  Hardware Identification of Customer 
        public string IP { get; set; }

       
        

    }
    public partial class License
    {
        private TimeSpan GetExpiryTimeSpan()
        {
            return ExpiryUnit switch
            {
                ExpiryUnit.Seconds => TimeSpan.FromSeconds(Duration),
                ExpiryUnit.Minutes => TimeSpan.FromMinutes(Duration),
                ExpiryUnit.Hours => TimeSpan.FromHours(Duration),
                ExpiryUnit.Days => TimeSpan.FromDays(Duration),
                ExpiryUnit.Months => TimeSpan.FromDays(30 * Duration), //  month duration
                ExpiryUnit.Years => TimeSpan.FromDays(365 * Duration), //  year duration
                _ => TimeSpan.Zero,
            };
        }

        //Set or add ExpiryDate
        public void AdjustExpiryDate()
        {
            ExpiryDate = CreationDate.Add(GetExpiryTimeSpan());
        }
        // Create License by Pattern
        public string GenerateCustomPattern()
        {
            var random = new Random();
            return new string(_pattern.Select(c => c == '*' ? (char)random.Next(65, 91) : c).ToArray());
        }
        //Adjust for Unfreeze License
        public void AdjustExpiryDateAfterUnfreeze()
        {
            if (FreezeStartTime.HasValue && FreezeEndTime.HasValue)
            {
                var frozenDuration = FreezeEndTime.Value - FreezeStartTime.Value;
                ExpiryDate = ExpiryDate.Add(frozenDuration); // Add frozen duration to expiry date
                FreezeStartTime = null;
                FreezeEndTime = null;
            }
        }
    }
}
