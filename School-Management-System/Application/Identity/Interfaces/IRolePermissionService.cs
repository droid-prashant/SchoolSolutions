using Application.Identity.Dtos;
using Application.Identity.ViewModel;

namespace Application.Identity.Interfaces
{
    public interface IRolePermissionService
    {
        Task<List<RoleViewModel>> GetRolesAsync(CancellationToken cancellationToken);
        Task<List<PermissionViewModel>> GetPermissionsAsync(CancellationToken cancellationToken);
        Task<RoleViewModel> GetRoleAsync(Guid roleId, CancellationToken cancellationToken);
        Task<RoleViewModel> CreateRoleAsync(RoleDto roleDto, CancellationToken cancellationToken);
        Task<RoleViewModel> UpdateRoleAsync(Guid roleId, RoleDto roleDto, CancellationToken cancellationToken);
        Task DeleteRoleAsync(Guid roleId, CancellationToken cancellationToken);
        Task SetRolePermissionsAsync(Guid roleId, RolePermissionsDto rolePermissionsDto, CancellationToken cancellationToken);
        Task<List<UserViewModel>> GetUsersAsync(CancellationToken cancellationToken);
        Task<UserRolesViewModel> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken);
        Task SetUserRolesAsync(Guid userId, UserRolesDto userRolesDto, CancellationToken cancellationToken);
    }
}
