using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Service.IService;
using System.Security.Claims;

namespace QodraTick.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "نام کاربری و رمز عبور الزامی است." });
            }

            // Authenticate user
            var user = await _userService.LoginAsync(request.Username, request.Password);

            if (user != null)
            {
                // Create claims
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
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = request.RememberMe,
                    ExpiresUtc = request.RememberMe ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddHours(1),
                    IssuedUtc = DateTimeOffset.UtcNow
                };

                // ✅ Sign in using HttpContext - این همیشه کار می‌کنه
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    claimsPrincipal,
                    authProperties);

                return Ok(new
                {
                    success = true,
                    message = "ورود موفقیت‌آمیز",
                    user = new
                    {
                        id = user.Id,
                        username = user.Username,
                        displayName = user.DisplayName,
                        role = user.Role.Name
                    }
                });
            }
            else
            {
                return Unauthorized(new { message = "نام کاربری یا رمز عبور اشتباه است." });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"خطا در ورود: {ex.Message}" });
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        try
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { success = true, message = "خروج موفقیت‌آمیز" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"خطا در خروج: {ex.Message}" });
        }
    }

    [HttpGet("validate")]
    public async Task<IActionResult> ValidateToken()
    {
        try
        {
            var user = await _userService.GetCurrentUserAsync();

            if (user != null && HttpContext.User.Identity?.IsAuthenticated == true)
            {
                return Ok(new
                {
                    success = true,
                    isValid = true,
                    user = new
                    {
                        id = user.Id,
                        username = user.Username,
                        displayName = user.DisplayName,
                        email = user.Email,
                        role = user.Role.Name
                    }
                });
            }
            else
            {
                return Unauthorized(new { success = false, isValid = false, message = "Token نامعتبر است" });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = $"خطا در اعتبارسنجی: {ex.Message}" });
        }
    }

    [HttpGet("user-info")]
    public async Task<IActionResult> GetCurrentUserInfo()
    {
        try
        {
            var user = await _userService.GetCurrentUserAsync();

            if (user != null)
            {
                return Ok(new
                {
                    success = true,
                    user = new
                    {
                        id = user.Id,
                        username = user.Username,
                        displayName = user.DisplayName,
                        email = user.Email,
                        role = user.Role.Name,
                        isActive = user.IsActive,
                        createdAt = user.CreatedAt
                    }
                });
            }
            else
            {
                return Unauthorized(new { success = false, message = "کاربر یافت نشد" });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = $"خطا در دریافت اطلاعات کاربر: {ex.Message}" });
        }
    }
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; } = false;
}
