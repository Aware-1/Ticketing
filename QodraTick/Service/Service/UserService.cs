using Data.Context;
using Data.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Service.IService;

namespace Service.Service
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<User?> GetCurrentUserAsync()
        {
            var username = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return null;

            return await GetUserByUsernameAsync(username);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);
        }

        public async Task<User> CreateOrUpdateUserAsync(string username, string displayName, string email)
        {
            var user = await GetUserByUsernameAsync(username);

            if (user == null)
            {
                user = new User
                {
                    Username = username,
                    DisplayName = displayName,
                    Role = UserRole.User // Default role
                };
                _context.Users.Add(user);
            }
            else
            {
                user.DisplayName = displayName;
                _context.Users.Update(user);
            }

            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> HasRoleAsync(string username, UserRole role)
        {
            var user = await GetUserByUsernameAsync(username);
            return user?.Role == role;
        }

        public async Task<List<User>> GetSupportUsersAsync()
        {
            return await _context.Users
                .Where(u => u.Role == UserRole.Support && u.IsActive)
                .ToListAsync();
        }

        public async Task<List<User>> GetAdminUsersAsync()
        {
            return await _context.Users
                .Where(u => u.Role == UserRole.Admin && u.IsActive)
                .ToListAsync();
        }
    }
}
