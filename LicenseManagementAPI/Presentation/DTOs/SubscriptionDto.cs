using System.ComponentModel.DataAnnotations;

namespace LicenseManagementAPI.Presentation.DTOs
{
    public class AddSubscriptionDto
    {
        [Required(ErrorMessage = "Application ID is required")]
        public int ApplicationId { get; set; }

        [Required(ErrorMessage = "Subscription name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Access level is required")]
        public int AccessLevel { get; set; }
    }
    public class SubscriptionDto
    {
        public string Name { get; set; }
        public int Level { get; set; }
    }
    public class DeleteSubscriptionDto
    {
        [Required(ErrorMessage = "Subscription ID is required")]
        public int SubscriptionId { get; set; }
    }
}
