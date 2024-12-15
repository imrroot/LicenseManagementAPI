using LicenseManagementAPI.Core.Entities;

namespace LicenseManagementAPI.Infrastructure.Interfaces
{
    public interface ILicenseRepository
    {
        Task<License> GetLicenseByUserAsync(string licenseKey,int userid);
        Task<License> GetLicenseByKeyAsync(string licenseKey);
        Task<IEnumerable<License>> GetLicensesByAppIdAsync(int appId);
        Task AddLicenseAsync(License licenses);
        Task AddLicenseAsync(IEnumerable<License> licenses);
        Task UpdateLicenseAsync(License license);
        Task DeleteLicenseAsync(License license);
        Task DeleteAllAsync(Func<License, bool> predicate = null);
    }
}
