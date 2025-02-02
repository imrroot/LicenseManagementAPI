using LicenseManagementAPI.Core.Entities;
using LicenseManagementAPI.Infrastructure.Interfaces;
using LicenseManagementAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LicenseManagementAPI.Infrastructure.Repositories;

public class AppRepository : IAppRepository
{
    private readonly LicenseDbContext _context;

    public AppRepository(LicenseDbContext context)
    {
        _context = context;
    }
    

    public async Task<App> GetAppByIdAsync(int appId, int userId)
    {
        return await _context.Apps.FirstOrDefaultAsync(x => x.Id == appId && x.UserId == userId);
    }

    public async Task<App> GetAppByIdWithSubscriptionsAsync(int appId, int userId)
    {
       return await _context.Apps.Include(s=>s.Subscriptions).FirstOrDefaultAsync(a=>a.Id == appId && a.UserId == userId);
    }

    public async Task<App> GetAppByIdWithLicensesAsync(int appId, int userId)
    {
        return await _context.Apps.Include(l => l.Licenses).FirstOrDefaultAsync(a => a.Id == appId && a.UserId == userId);
    }

    public async Task<App> GetAppByKeyAsync(string appKey)
    {
        return await _context.Apps.Include(a => a.User).FirstOrDefaultAsync(a => a.AppKey == appKey);
    }

    public async Task<IEnumerable<App>> GetAppsByUserIdAsync(int userId)
    {
        return await _context.Apps.Include(l=>l.Licenses).Where(a => a.UserId == userId).ToListAsync();
    }

    public async Task AddAppAsync(App app)
    {
        try
        {
            _context.Apps.Add(app);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            
        }
    }

    public async Task UpdateAppAsync(App app)
    {
        try
        {
            _context.Apps.Update(app);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            
        }
    }

    public async Task DeleteAppAsync(App app)
    {
        try
        {
            _context.Apps.Remove(app);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            
        }
    }
}