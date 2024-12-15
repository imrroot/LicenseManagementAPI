using LicenseManagementAPI.Core.Entities;
using LicenseManagementAPI.Infrastructure.Interfaces;
using LicenseManagementAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LicenseManagementAPI.Infrastructure.Repositories;

public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly LicenseDbContext _context;

    public SubscriptionRepository(LicenseDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Subscription>> GetSubscriptionsByAppIdAsync(int appId)
    {
        return await _context.Subscriptions.Where(s => s.AppId == appId).ToListAsync();
    }

    public async Task<Subscription> GetSubscriptionByIdAsync(int subscriptionId, int userId)
    {
       return await _context.Subscriptions.FirstOrDefaultAsync(s=>s.Id == subscriptionId && s.AppId == userId);
    }

    public async Task AddSubscriptionAsync(Subscription subscription)
    {
        try
        {
            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            
        }
    }


    public async Task DeleteSubscriptionAsync(Subscription subscription)
    {
        try
        {
            _context.Subscriptions.Remove(subscription);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            
        }
        
    }
}