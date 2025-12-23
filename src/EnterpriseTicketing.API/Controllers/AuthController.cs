using Microsoft.AspNetCore.Mvc;
using EnterpriseTicketing.Application.Interfaces;
using EnterpriseTicketing.Domain.Entities;
using EnterpriseTicketing.Infrastructure.Services;

namespace EnterpriseTicketing.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IIdentityService _identityService;

    public AuthController(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Department = request.Department,
            PhoneNumber = request.PhoneNumber,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _identityService.CreateUserAsync(user, request.Password);
        if (!result.Success)
        {
            return BadRequest(new { Errors = result.Errors });
        }

        return Ok(new { Message = "User registered successfully" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var isValid = await _identityService.ValidateCredentialsAsync(request.Email, request.Password);
        if (!isValid)
        {
            return Unauthorized(new { Message = "Invalid credentials" });
        }

        var user = await _identityService.GetUserByEmailAsync(request.Email);
        var token = await _identityService.GenerateJwtTokenAsync(user);

        return Ok(new { Token = token });
    }
}

public class RegisterRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Department { get; set; }
    public string PhoneNumber { get; set; }
}

public class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}