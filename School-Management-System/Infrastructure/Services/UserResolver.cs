using Microsoft.AspNetCore.Http;
using System;
using Infrastructure.Identity;
using System.Linq;
using System.Security.Claims;

namespace Infrastructure.Services;
public class UserResolver
{
    public readonly string UserId;
    public readonly string UserName;
    public readonly string Email;
    public readonly string AcademicYearId;
    public readonly string Role;
    public readonly IReadOnlyList<string> Permissions = Array.Empty<string>();
    public readonly bool IsSuperAdmin;
    public UserResolver(IHttpContextAccessor httpCtx)
    {
        var user = httpCtx?.HttpContext?.User;

        if (user == null || !user.Identity?.IsAuthenticated == true)
            return;

        UserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        Email = user.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
        var roles = user.FindAll(ClaimTypes.Role)
            .Concat(user.FindAll("roles"))
            .Select(x => x.Value)
            .ToList();

        Role = roles.FirstOrDefault() ?? string.Empty;
        IsSuperAdmin = roles.Any(x =>
            x.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase) ||
            x.Equals("Super Admin", StringComparison.OrdinalIgnoreCase));

        Permissions = user.FindAll(CustomClaimType.Permission)
            .Select(x => x.Value)
            .ToList();

        UserName = user.FindFirst("name")?.Value ?? string.Empty;
        AcademicYearId = user.FindFirst("academicYear")?.Value ?? string.Empty;
    }

    public Guid GetAcademicYearGuidOrThrow()
    {
        if (!Guid.TryParse(AcademicYearId, out var academicYearGuid))
        {
            throw new InvalidOperationException("Academic year is not available in the current session.");
        }

        return academicYearGuid;
    }

    public bool HasAnyPermission(params string[] permissions)
    {
        if (IsSuperAdmin || permissions.Length == 0)
        {
            return true;
        }

        return permissions.Any(permission =>
            Permissions.Any(grantedPermission =>
                grantedPermission.Equals(permission, StringComparison.OrdinalIgnoreCase)));
    }
}
