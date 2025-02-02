using System.ComponentModel.DataAnnotations;

namespace LicenseManagementAPI.Presentation.DTOs
{
    public class ApplicationWithSubscriptionsResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AppKey { get; set; }
        public string AppSecret { get; set; }
        public int TotalLicenses { get; set; }
        public int ActiveLicenses { get; set; }
        public int NotUsedLicenses { get; set; }
        public int BannedLicenses { get; set; }
        public int FrozenLicenses { get; set; }
        public bool IsFrozen { get; set; }
        public List<SubscriptionDto> Subscriptions { get; set; } = new List<SubscriptionDto>();
    }
    
    public class CreateApplicationDto
    {
        [Required(ErrorMessage = "Application name is required")]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }
    }
    
}
