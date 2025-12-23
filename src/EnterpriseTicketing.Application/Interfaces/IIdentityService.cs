using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using EnterpriseTicketing.Domain.Entities;

namespace EnterpriseTicketing.Infrastructure.Services;

public interface IIdentityService
{
    Task<(bool Success, string[] Errors)> CreateUserAsync(User user, string password);
    Task<(bool Success, string[] Errors)> DeleteUserAsync(User user);
    Task<User> GetUserByIdAsync(Guid userId);
    Task<User> GetUserByEmailAsync(string email);
    Task<bool> ValidateCredentialsAsync(string email, string password);
    Task<string> GenerateJwtTokenAsync(User user);
    Task<IList<string>> GetUserRolesAsync(User user);
}