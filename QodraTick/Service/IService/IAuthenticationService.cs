using Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IAuthenticationService
    {
        Task<bool> SignInAsync(User user, bool rememberMe);
        Task SignOutAsync();
        Task<User?> GetCurrentUserAsync();
        ClaimsPrincipal CreateClaimsPrincipal(User user);
    }
}
