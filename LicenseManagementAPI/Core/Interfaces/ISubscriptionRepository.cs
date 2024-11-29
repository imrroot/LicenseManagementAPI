using LicenseManagementAPI.Core.Entities;

namespace LicenseManagementAPI.Core.Interfaces
{
    public interface ISubscriptionRepository
    {
        Task<IEnumerable<Subscription>> GetSubscriptionsByAppIdAsync(int appId);
        Task<Subscription> GetSubscriptionByIdAsync(int subscriptionId, int userId);
        Task AddSubscriptionAsync(Subscription subscription);
        Task DeleteSubscriptionAsync(Subscription subscription);
    }
}
