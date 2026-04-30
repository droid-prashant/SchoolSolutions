using Application.Identity.Dtos;
using Application.Identity.Interfaces;
using Application.Identity.ViewModel;
using Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;

namespace WebApi.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IRolePermissionService _rolePermissionService;

        public UsersController(IRolePermissionService rolePermissionService)
        {
            _rolePermissionService = rolePermissionService;
        }

        [HttpGet]
        [HasPermission(PermissionNames.UserManage)]
        public async Task<List<UserViewModel>> GetUsers(CancellationToken cancellationToken)
        {
            return await _rolePermissionService.GetUsersAsync(cancellationToken);
        }

        [HttpGet("{id:guid}/roles")]
        [HasPermission(PermissionNames.UserManage)]
        public async Task<UserRolesViewModel> GetUserRoles([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            return await _rolePermissionService.GetUserRolesAsync(id, cancellationToken);
        }

        [HttpPost("{id:guid}/roles")]
        [HasPermission(PermissionNames.UserManage)]
        public async Task SetUserRoles([FromRoute] Guid id, [FromBody] UserRolesDto userRolesDto, CancellationToken cancellationToken)
        {
            await _rolePermissionService.SetUserRolesAsync(id, userRolesDto, cancellationToken);
        }
    }
}
