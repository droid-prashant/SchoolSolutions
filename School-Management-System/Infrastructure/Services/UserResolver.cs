using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace Infrastructure.Services;
public class UserResolver
{
    public readonly string UserId;
    public readonly string UserName;
    public readonly string Email;
    public readonly string AcademicYearId;
    public readonly string Role;
    public readonly bool IsSuperAdmin;
    public UserResolver(IHttpContextAccessor httpCtx)
    {
        var user = httpCtx?.HttpContext?.User;

        if (user == null || !user.Identity?.IsAuthenticated == true)
            return;

        UserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        Email = user.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
        Role = user.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

        UserName = user.FindFirst("name")?.Value ?? string.Empty;
        AcademicYearId = user.FindFirst("academicYear")?.Value ?? string.Empty;
    }

    public bool HasPermission(string permissionName)
    {
        // Add logic here later
        return false;
    }
}