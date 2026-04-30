using System.Security.Claims;
using Domain;
using Domain.Constants;
using Infrastructure.Identity;
using Infrastructure.Persistance;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Seeds
{
    public static class DefaultRoles
    {
        public static async Task SeedRolesAsync(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ApplicationDbContext dbContext)
        {
            var roles = new[]
            {
                ("SuperAdmin", "System owner with all permissions."),
                ("Admin", "School administrator."),
                ("Principal", "Principal with academic oversight permissions."),
                ("Teacher", "Teacher with assigned teaching permissions."),
                ("Accountant", "Accountant with fee management permissions.")
            };

            foreach (var (roleName, description) in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new ApplicationRole
                    {
                        Name = roleName,
                        Description = description
                    });
                }
            }

            await SeedPermissionsAsync(dbContext);
            await AssignAllPermissionsToSuperAdmin(roleManager, dbContext);
        }

        private static async Task SeedPermissionsAsync(ApplicationDbContext dbContext)
        {
            foreach (var permissionDefinition in PermissionNames.All)
            {
                var permission = await dbContext.Permissions
                    .FirstOrDefaultAsync(x => x.Code == permissionDefinition.Code);

                if (permission == null)
                {
                    await dbContext.Permissions.AddAsync(new Permission
                    {
                        Id = Guid.NewGuid(),
                        Code = permissionDefinition.Code,
                        Name = permissionDefinition.Name,
                        GroupName = permissionDefinition.GroupName,
                        Description = permissionDefinition.Description,
                        IsActive = true
                    });
                    continue;
                }

                permission.Name = permissionDefinition.Name;
                permission.GroupName = permissionDefinition.GroupName;
                permission.Description = permissionDefinition.Description;
                permission.IsActive = true;
            }

            await dbContext.SaveChangesAsync(CancellationToken.None);
        }

        private static async Task AssignAllPermissionsToSuperAdmin(RoleManager<ApplicationRole> roleManager, ApplicationDbContext dbContext)
        {
            var superAdminRole = await roleManager.FindByNameAsync("SuperAdmin");
            if (superAdminRole == null)
            {
                return;
            }

            var permissions = await dbContext.Permissions.ToListAsync();
            foreach (var permission in permissions)
            {
                var rolePermissionExists = await dbContext.RolePermissions
                    .AnyAsync(x => x.RoleId == superAdminRole.Id && x.PermissionId == permission.Id);
                if (!rolePermissionExists)
                {
                    await dbContext.RolePermissions.AddAsync(new RolePermission
                    {
                        Id = Guid.NewGuid(),
                        RoleId = superAdminRole.Id,
                        PermissionId = permission.Id,
                        IsActive = true
                    });
                }
            }

            await dbContext.SaveChangesAsync(CancellationToken.None);

            var existingPermissionClaims = (await roleManager.GetClaimsAsync(superAdminRole))
                .Where(x => x.Type == CustomClaimType.Permission)
                .ToList();

            foreach (var permission in permissions)
            {
                if (existingPermissionClaims.Any(x => x.Value == permission.Code))
                {
                    continue;
                }

                await roleManager.AddClaimAsync(superAdminRole, new Claim(CustomClaimType.Permission, permission.Code));
            }
        }
    }
}
