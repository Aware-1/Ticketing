using Data.Model;

namespace Service.IService
{
    public interface IUserService
    {
        Task<User?> GetCurrentUserAsync();
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User> CreateOrUpdateUserAsync(string username, string displayName, string email);
        Task<bool> HasRoleAsync(string username, UserRole role);
        Task<List<User>> GetSupportUsersAsync();
        Task<List<User>> GetAdminUsersAsync();
    }
}
