using System.Security.Claims;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class HasPermissionAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string[] _permissions;

        public HasPermissionAttribute(params string[] permissions)
        {
            _permissions = permissions ?? [];
        }

        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (user.Identity?.IsAuthenticated != true)
            {
                context.Result = new UnauthorizedResult();
                return Task.CompletedTask;
            }

            if (_permissions.Length == 0 || IsSuperAdmin(user))
            {
                return Task.CompletedTask;
            }

            var grantedPermissions = user.FindAll(CustomClaimType.Permission)
                .Select(x => x.Value)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            if (_permissions.Any(grantedPermissions.Contains))
            {
                return Task.CompletedTask;
            }

            context.Result = new ForbidResult();
            return Task.CompletedTask;
        }

        private static bool IsSuperAdmin(ClaimsPrincipal user)
        {
            return user.IsInRole("SuperAdmin") ||
                   user.IsInRole("Super Admin") ||
                   user.FindAll(ClaimTypes.Role).Any(x => x.Value.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase)) ||
                   user.FindAll("roles").Any(x => x.Value.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase));
        }
    }
}
