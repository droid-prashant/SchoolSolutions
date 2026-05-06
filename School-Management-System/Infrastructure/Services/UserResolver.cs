using Microsoft.AspNetCore.Http;
using System;
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
}
