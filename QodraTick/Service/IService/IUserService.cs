using Data.Model;

namespace Service.IService
{
    public interface IUserService
    {
        Task<User?> GetCurrentUserAsync();
        Task<User?> LoginAsync(string username, string password);
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<List<User>> GetSupportUsersAsync();
        Task<List<User>> GetAdminUsersAsync();
        Task<List<Role>> GetRolesAsync();
    }
}
