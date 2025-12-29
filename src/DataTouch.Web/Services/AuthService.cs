using DataTouch.Domain.Entities;
using DataTouch.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DataTouch.Web.Services;

public class AuthService
{
    private readonly DataTouchDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(DataTouchDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<User?> ValidateUserAsync(string email, string password)
    {
        var passwordHash = HashPassword(password);
        var user = await _dbContext.Users
            .Include(u => u.Organization)
            .FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == passwordHash && u.IsActive);

        return user;
    }

    public async Task SignInAsync(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Role, user.Role),
            new("OrganizationId", user.OrganizationId.ToString()),
            new("OrganizationName", user.Organization?.Name ?? "")
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
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

    public static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    public Guid? GetCurrentUserId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var claim = httpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? Guid.Parse(claim.Value) : null;
    }

    public Guid? GetCurrentOrganizationId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var claim = httpContext?.User.FindFirst("OrganizationId");
        return claim != null ? Guid.Parse(claim.Value) : null;
    }

    public string? GetCurrentEmail()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var claim = httpContext?.User.FindFirst(ClaimTypes.Email);
        return claim?.Value;
    }
}
