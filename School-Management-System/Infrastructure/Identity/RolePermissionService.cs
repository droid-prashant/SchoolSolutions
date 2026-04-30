using System.Security.Claims;
using Application.Identity.Dtos;
using Application.Identity.Interfaces;
using Application.Identity.ViewModel;
using Domain;
using Infrastructure.Persistance;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Identity
{
    public class RolePermissionService : IRolePermissionService
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RolePermissionService(
            ApplicationDbContext context,
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<List<RoleViewModel>> GetRolesAsync(CancellationToken cancellationToken)
        {
            var roles = await _roleManager.Roles
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);

            var roleIds = roles.Select(x => x.Id).ToList();
            var permissionsByRole = await GetPermissionCodesByRoleIds(roleIds, cancellationToken);

            return roles.Select(role => MapRole(role, permissionsByRole.GetValueOrDefault(role.Id, []))).ToList();
        }

        public async Task<List<PermissionViewModel>> GetPermissionsAsync(CancellationToken cancellationToken)
        {
            return await _context.Permissions
                .AsNoTracking()
                .OrderBy(x => x.GroupName)
                .ThenBy(x => x.Name)
                .Select(x => new PermissionViewModel
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    GroupName = x.GroupName,
                    Description = x.Description
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<RoleViewModel> GetRoleAsync(Guid roleId, CancellationToken cancellationToken)
        {
            var role = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Id == roleId, cancellationToken);
            if (role == null)
            {
                throw new Exception("Role not found.");
            }

            var permissionCodes = await GetRolePermissionCodes(roleId, cancellationToken);
            return MapRole(role, permissionCodes);
        }

        public async Task<RoleViewModel> CreateRoleAsync(RoleDto roleDto, CancellationToken cancellationToken)
        {
            ValidateRole(roleDto);
            var roleName = roleDto.Name.Trim();
            if (await _roleManager.RoleExistsAsync(roleName))
            {
                throw new Exception("A role with this name already exists.");
            }

            var role = new ApplicationRole
            {
                Id = Guid.NewGuid(),
                Name = roleName,
                Description = roleDto.Description?.Trim() ?? string.Empty
            };

            var result = await _roleManager.CreateAsync(role);
            EnsureIdentityResult(result, "Failed to create role.");

            await SetRolePermissionsInternalAsync(role.Id, roleDto.PermissionCodes, cancellationToken);
            return await GetRoleAsync(role.Id, cancellationToken);
        }

        public async Task<RoleViewModel> UpdateRoleAsync(Guid roleId, RoleDto roleDto, CancellationToken cancellationToken)
        {
            ValidateRole(roleDto);
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
            {
                throw new Exception("Role not found.");
            }

            var roleName = roleDto.Name.Trim();
            var duplicateRole = await _roleManager.Roles
                .AnyAsync(x => x.Id != roleId && x.NormalizedName == roleName.ToUpper(), cancellationToken);
            if (duplicateRole)
            {
                throw new Exception("A role with this name already exists.");
            }

            role.Name = roleName;
            role.Description = roleDto.Description?.Trim() ?? string.Empty;
            var result = await _roleManager.UpdateAsync(role);
            EnsureIdentityResult(result, "Failed to update role.");

            await SetRolePermissionsInternalAsync(role.Id, roleDto.PermissionCodes, cancellationToken);
            return await GetRoleAsync(role.Id, cancellationToken);
        }

        public async Task DeleteRoleAsync(Guid roleId, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
            {
                throw new Exception("Role not found.");
            }

            if (IsSuperAdminRole(role.Name))
            {
                throw new Exception("SuperAdmin role cannot be deleted.");
            }

            var isAssigned = await _context.UserRoles.AnyAsync(x => x.RoleId == roleId, cancellationToken);
            if (isAssigned)
            {
                throw new Exception("This role is assigned to one or more users and cannot be deleted.");
            }

            var result = await _roleManager.DeleteAsync(role);
            EnsureIdentityResult(result, "Failed to delete role.");
        }

        public async Task SetRolePermissionsAsync(Guid roleId, RolePermissionsDto rolePermissionsDto, CancellationToken cancellationToken)
        {
            var roleExists = await _roleManager.Roles.AnyAsync(x => x.Id == roleId, cancellationToken);
            if (!roleExists)
            {
                throw new Exception("Role not found.");
            }

            await SetRolePermissionsInternalAsync(roleId, rolePermissionsDto.PermissionCodes, cancellationToken);
        }

        public async Task<List<UserViewModel>> GetUsersAsync(CancellationToken cancellationToken)
        {
            var users = await _userManager.Users
                .OrderBy(x => x.UserName)
                .ToListAsync(cancellationToken);

            var result = new List<UserViewModel>();
            foreach (var user in users)
            {
                result.Add(new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    FullName = string.Join(" ", new[] { user.FirstName, user.LastName }.Where(x => !string.IsNullOrWhiteSpace(x))),
                    IsActive = user.IsActive,
                    Roles = (await _userManager.GetRolesAsync(user)).ToList()
                });
            }

            return result;
        }

        public async Task<UserRolesViewModel> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            return new UserRolesViewModel
            {
                UserId = user.Id,
                UserName = user.UserName ?? string.Empty,
                RoleNames = (await _userManager.GetRolesAsync(user)).ToList()
            };
        }

        public async Task SetUserRolesAsync(Guid userId, UserRolesDto userRolesDto, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            var requestedRoles = userRolesDto.RoleNames
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var missingRoles = new List<string>();
            foreach (var role in requestedRoles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    missingRoles.Add(role);
                }
            }

            if (missingRoles.Count > 0)
            {
                throw new Exception($"Role(s) not found: {string.Join(", ", missingRoles)}.");
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            EnsureIdentityResult(removeResult, "Failed to remove existing user roles.");

            if (requestedRoles.Count == 0)
            {
                return;
            }

            var addResult = await _userManager.AddToRolesAsync(user, requestedRoles);
            EnsureIdentityResult(addResult, "Failed to assign user roles.");
        }

        private async Task SetRolePermissionsInternalAsync(Guid roleId, List<string> permissionCodes, CancellationToken cancellationToken)
        {
            var cleanCodes = permissionCodes
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var permissions = await _context.Permissions
                .Where(x => cleanCodes.Contains(x.Code))
                .ToListAsync(cancellationToken);

            var missingCodes = cleanCodes
                .Where(code => permissions.All(permission => !permission.Code.Equals(code, StringComparison.OrdinalIgnoreCase)))
                .ToList();
            if (missingCodes.Count > 0)
            {
                throw new Exception($"Permission(s) not found: {string.Join(", ", missingCodes)}.");
            }

            var existing = await _context.RolePermissions
                .Where(x => x.RoleId == roleId)
                .ToListAsync(cancellationToken);
            if (existing.Count > 0)
            {
                _context.RolePermissions.RemoveRange(existing);
            }

            foreach (var permission in permissions)
            {
                await _context.RolePermissions.AddAsync(new RolePermission
                {
                    Id = Guid.NewGuid(),
                    RoleId = roleId,
                    PermissionId = permission.Id,
                    IsActive = true
                }, cancellationToken);
            }

            await _context.SaveChangesAsync(cancellationToken);
            await SyncRolePermissionClaims(roleId, permissions.Select(x => x.Code).ToList());
        }

        private async Task SyncRolePermissionClaims(Guid roleId, List<string> permissionCodes)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
            {
                throw new Exception("Role not found.");
            }

            var existingClaims = await _roleManager.GetClaimsAsync(role);
            foreach (var claim in existingClaims.Where(x => x.Type == CustomClaimType.Permission))
            {
                var removeResult = await _roleManager.RemoveClaimAsync(role, claim);
                EnsureIdentityResult(removeResult, "Failed to remove existing role permission claims.");
            }

            foreach (var permissionCode in permissionCodes)
            {
                var addResult = await _roleManager.AddClaimAsync(role, new Claim(CustomClaimType.Permission, permissionCode));
                EnsureIdentityResult(addResult, "Failed to add role permission claim.");
            }
        }

        private async Task<List<string>> GetRolePermissionCodes(Guid roleId, CancellationToken cancellationToken)
        {
            return await _context.RolePermissions
                .AsNoTracking()
                .Where(x => x.RoleId == roleId)
                .OrderBy(x => x.Permission.GroupName)
                .ThenBy(x => x.Permission.Name)
                .Select(x => x.Permission.Code)
                .ToListAsync(cancellationToken);
        }

        private async Task<Dictionary<Guid, List<string>>> GetPermissionCodesByRoleIds(List<Guid> roleIds, CancellationToken cancellationToken)
        {
            return await _context.RolePermissions
                .AsNoTracking()
                .Where(x => roleIds.Contains(x.RoleId))
                .GroupBy(x => x.RoleId)
                .Select(x => new
                {
                    RoleId = x.Key,
                    PermissionCodes = x.Select(rp => rp.Permission.Code).ToList()
                })
                .ToDictionaryAsync(x => x.RoleId, x => x.PermissionCodes, cancellationToken);
        }

        private static RoleViewModel MapRole(ApplicationRole role, List<string> permissionCodes)
        {
            return new RoleViewModel
            {
                Id = role.Id,
                Name = role.Name ?? string.Empty,
                Description = role.Description ?? string.Empty,
                PermissionCodes = permissionCodes
            };
        }

        private static void ValidateRole(RoleDto roleDto)
        {
            if (string.IsNullOrWhiteSpace(roleDto.Name))
            {
                throw new Exception("Role name is required.");
            }

            if (roleDto.Name.Length > 100)
            {
                throw new Exception("Role name cannot exceed 100 characters.");
            }

            if (roleDto.Description?.Length > 300)
            {
                throw new Exception("Role description cannot exceed 300 characters.");
            }
        }

        private static void EnsureIdentityResult(IdentityResult result, string message)
        {
            if (result.Succeeded)
            {
                return;
            }

            var errors = result.Errors.Select(x => x.Description).ToList();
            throw new Exception(errors.Count > 0 ? string.Join(", ", errors) : message);
        }

        private static bool IsSuperAdminRole(string? roleName)
        {
            return roleName?.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase) == true ||
                   roleName?.Equals("Super Admin", StringComparison.OrdinalIgnoreCase) == true;
        }
    }
}
