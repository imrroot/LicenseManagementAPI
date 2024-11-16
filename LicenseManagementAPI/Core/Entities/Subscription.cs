namespace LicenseManagementAPI.Core.Entities
{
    public class Subscription
    {
        public int Id { get; set; }
        public string Name { get; set; } //Access Level Name e.g., "Booster", "Premium"
        public int Level { get; set; } //Integer of Access Level
        public int AppId { get; set; } //Foreign Key to Application
        public App App { get; set; }
    }
}
