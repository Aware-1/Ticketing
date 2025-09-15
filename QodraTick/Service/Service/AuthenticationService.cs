using Data.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Service.IService;
using System.Security.Claims;
using AuthenticationProperties = Microsoft.AspNetCore.Authentication.AuthenticationProperties;
using IAuthenticationService = Service.IService.IAuthenticationService;

namespace Service.Service;

public class AuthenticationService : IAuthenticationService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserService _userService;

    public AuthenticationService(IHttpContextAccessor httpContextAccessor, IUserService userService)
    {
        _httpContextAccessor = httpContextAccessor;
        _userService = userService;
    }

    public async Task<bool> SignInAsync(User user, bool rememberMe)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
            return false;

        try
        {
            var claimsPrincipal = CreateClaimsPrincipal(user);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = rememberMe,
                ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddHours(1)
            };

            await httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal,
                authProperties);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task SignOutAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }

    public async Task<User?> GetCurrentUserAsync()
    {
        return await _userService.GetCurrentUserAsync();
    }

    public ClaimsPrincipal CreateClaimsPrincipal(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("DisplayName", user.DisplayName),
            new Claim(ClaimTypes.Role, user.Role.Name),
            new Claim("UserId", user.Id.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        return new ClaimsPrincipal(claimsIdentity);
    }
}
