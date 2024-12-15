using LicenseManagementAPI.Core.Entities;
using LicenseManagementAPI.Infrastructure.Interfaces;
using LicenseManagementAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LicenseManagementAPI.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly LicenseDbContext _context;

    public UserRepository(LicenseDbContext context)
    {
        _context = context;
    }

    public async Task<User> GetUserByIdAsync(int userId)
    {
        return await _context.Users.FindAsync(userId);
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task AddUserAsync(User user)
    {
        try
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            
        }
    }

    public async Task UpdateUserAsync(User user)
    {
        try
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            
        }
    }
}
