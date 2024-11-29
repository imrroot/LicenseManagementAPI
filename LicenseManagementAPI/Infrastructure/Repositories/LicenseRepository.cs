using LicenseManagementAPI.Core.Entities;
using LicenseManagementAPI.Core.Interfaces;
using LicenseManagementAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LicenseManagementAPI.Infrastructure.Repositories;

public class LicenseRepository : ILicenseRepository
{
    private readonly LicenseDbContext _context;
    
    public LicenseRepository(LicenseDbContext context)
    {
        _context = context;
    }
    public async Task<License> GetLicenseByUserAsync(string licenseKey, int userid)
    {
        return await _context.Licenses.Include(a=>a.App).FirstOrDefaultAsync(l=>l.Pattern == licenseKey && l.App.UserId == userid);
    }

    public async Task<License> GetLicenseByKeyAsync(string licenseKey)
    {
        return await _context.Licenses.Include(l => l.App).FirstOrDefaultAsync(l => l.Pattern == licenseKey);
    }

    public async Task<IEnumerable<License>> GetLicensesByAppIdAsync(int appId)
    {
        return await _context.Licenses.Where(l => l.ApplicationId == appId).ToListAsync();
    }

    public async Task AddLicenseAsync(License license)
    {
        try
        {
            _context.Licenses.Add(license);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            
        }
    }

    public async Task UpdateLicenseAsync(License license)
    {
        try
        {
            _context.Licenses.Update(license);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            
        }
    }

    public async Task DeleteLicenseAsync(License license)
    {

        try
        {
            _context.Licenses.Remove(license);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            
        }
        
    }

   
    
}