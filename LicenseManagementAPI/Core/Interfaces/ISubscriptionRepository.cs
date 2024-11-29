using LicenseManagementAPI.Core.Entities;

namespace LicenseManagementAPI.Core.Interfaces
{
    public interface ISubscriptionRepository
    {
        Task<IEnumerable<Subscription>> GetSubscriptionsByAppIdAsync(int appId);
        Task AddSubscriptionAsync(Subscription subscription);
        Task<(bool Success, string message)> DeleteSubscriptionAsync(int subscriptionId,int userId);
    }
}
