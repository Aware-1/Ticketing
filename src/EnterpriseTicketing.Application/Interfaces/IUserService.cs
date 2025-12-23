using EnterpriseTicketing.Domain.Entities;

namespace EnterpriseTicketing.Application.Interfaces;

public interface IUserService
{
    Task<User?> GetUserByIdAsync(Guid id);
    Task<User?> GetUserByEmailAsync(string email);
    Task<IEnumerable<User>> GetUsersByDepartmentAsync(string department);
    Task<User> CreateUserAsync(User user, string password);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(Guid id);
    Task<bool> ValidateCredentialsAsync(string email, string password);
    Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
    Task<bool> IsEmailUniqueAsync(string email);
}