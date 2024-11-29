using System.ComponentModel.DataAnnotations;

namespace LicenseManagementAPI.Presentation.DTOs
{
    public class ApplicationResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TotalLicenses { get; set; }
        public int ActiveLicenses { get; set; }
    }
    public class CreateApplicationDto
    {
        [Required(ErrorMessage = "Application name is required")]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }
    }
    public class DeleteApplicationDto
    {
        [Required(ErrorMessage = "Application ID is required")]
        public int ApplicationId { get; set; }
    }
}
