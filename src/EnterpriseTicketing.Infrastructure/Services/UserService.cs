using Microsoft.EntityFrameworkCore;
using EnterpriseTicketing.Application.Interfaces;
using EnterpriseTicketing.Domain.Entities;
using EnterpriseTicketing.Infrastructure.Persistence;

namespace EnterpriseTicketing.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<IEnumerable<User>> GetUsersByDepartmentAsync(string department)
    {
        return await _context.Users
            .Where(u => u.Department == department)
            .ToListAsync();
    }

    public async Task<User> CreateUserAsync(User user, string password)
    {
        // TODO: Implement password hashing
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(Guid id)
    {
        var user = await GetUserByIdAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ValidateCredentialsAsync(string email, string password)
    {
        // TODO: Implement proper password validation
        var user = await GetUserByEmailAsync(email);
        return user != null;
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
    {
        // TODO: Implement password change logic
        var user = await GetUserByIdAsync(userId);
        return user != null;
    }

    public async Task<bool> IsEmailUniqueAsync(string email)
    {
        return !await _context.Users.AnyAsync(u => u.Email == email);
    }
}