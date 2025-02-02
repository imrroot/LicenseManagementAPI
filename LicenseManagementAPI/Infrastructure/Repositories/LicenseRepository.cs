﻿using LicenseManagementAPI.Core.Entities;
using LicenseManagementAPI.Infrastructure.Interfaces;
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
        return await _context.Licenses.Include(a => a.App).Include(s=>s.Subscription).FirstOrDefaultAsync(l => l.Pattern == licenseKey);
    }

    public async Task<IEnumerable<License>> GetLicensesByAppIdAsync(int appId)
    {
        return await _context.Licenses.Where(l => l.ApplicationId == appId).ToListAsync();
    }
    public async Task AddLicenseAsync(License licenses)
    {
        try
        {
            _context.Licenses.Add(licenses);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {

        }
    }
    public async Task AddLicenseAsync(IEnumerable<License> licenses)
    {
        try
        {
            _context.Licenses.AddRange(licenses);
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

    public async Task DeleteAllAsync(Func<License, bool> predicate = null)
    {
        try
        {
            var licensesToDelete = predicate == null
                ? _context.Licenses.ToList()
                : _context.Licenses.Where(predicate).ToList();

            if (licensesToDelete.Any())
            {
                _context.Licenses.RemoveRange(licensesToDelete);
                await _context.SaveChangesAsync();
            }
        }
        catch (DbUpdateException ex)
        {
            
        }
    }
}