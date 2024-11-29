using LicenseManagementAPI.Core.Entities;

namespace LicenseManagementAPI.Core.Interfaces
{
    public interface IAppRepository 
    {
        Task<App> GetAppByIdAsync(int appId,int userId);
        Task<App> GetAppByIdWithSubscriptionsAsync(int appId, int userId);
        Task<App> GetAppByKeyAsync(string appKey);
        Task<IEnumerable<App>> GetAppsByUserIdAsync(int userId);
        Task AddAppAsync(App app);
        Task UpdateAppAsync(App app);
        Task DeleteAppAsync(App app);
    }

}
