using Application.Identity.Dtos;
using Application.Identity.Interfaces;
using Application.Identity.ViewModel;
using Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;

namespace WebApi.Controllers
{
    [Route("api/roles")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRolePermissionService _rolePermissionService;

        public RolesController(IRolePermissionService rolePermissionService)
        {
            _rolePermissionService = rolePermissionService;
        }

        [HttpGet]
        [HasPermission(PermissionNames.RoleManage, PermissionNames.UserManage)]
        public async Task<List<RoleViewModel>> GetRoles(CancellationToken cancellationToken)
        {
            return await _rolePermissionService.GetRolesAsync(cancellationToken);
        }

        [HttpGet("/api/permissions")]
        [HasPermission(PermissionNames.RoleManage, PermissionNames.UserManage)]
        public async Task<List<PermissionViewModel>> GetPermissions(CancellationToken cancellationToken)
        {
            return await _rolePermissionService.GetPermissionsAsync(cancellationToken);
        }

        [HttpGet("{id:guid}")]
        [HasPermission(PermissionNames.RoleManage)]
        public async Task<RoleViewModel> GetRole([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            return await _rolePermissionService.GetRoleAsync(id, cancellationToken);
        }

        [HttpPost]
        [HasPermission(PermissionNames.RoleManage)]
        public async Task<RoleViewModel> CreateRole([FromBody] RoleDto roleDto, CancellationToken cancellationToken)
        {
            return await _rolePermissionService.CreateRoleAsync(roleDto, cancellationToken);
        }

        [HttpPut("{id:guid}")]
        [HasPermission(PermissionNames.RoleManage)]
        public async Task<RoleViewModel> UpdateRole([FromRoute] Guid id, [FromBody] RoleDto roleDto, CancellationToken cancellationToken)
        {
            return await _rolePermissionService.UpdateRoleAsync(id, roleDto, cancellationToken);
        }

        [HttpDelete("{id:guid}")]
        [HasPermission(PermissionNames.RoleManage)]
        public async Task DeleteRole([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            await _rolePermissionService.DeleteRoleAsync(id, cancellationToken);
        }

        [HttpPost("{id:guid}/permissions")]
        [HasPermission(PermissionNames.RoleManage)]
        public async Task SetRolePermissions([FromRoute] Guid id, [FromBody] RolePermissionsDto rolePermissionsDto, CancellationToken cancellationToken)
        {
            await _rolePermissionService.SetRolePermissionsAsync(id, rolePermissionsDto, cancellationToken);
        }
    }
}
