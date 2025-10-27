using Application.Identity.Dtos;
using Application.Identity.Interfaces;
using Application.Identity.ViewModel;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        public AppController(IIdentityService identityService)
        {
            _identityService = identityService;
        }
        [HttpPost("CreateRole")]
        public async Task CreateRole([FromBody] UserRoleDto userRoleDto, CancellationToken cancellationToken)
        {
            await _identityService.CreateRoleAsync(userRoleDto, cancellationToken);
        }
        [HttpPost("CreateUser")]
        public async Task CreateUser([FromBody] UserDto userDto, CancellationToken cancellationToken)
        {
            await _identityService.CreateUserAsync(userDto, cancellationToken);
        }
        [HttpPost("Login")]
        public async Task<TokenViewModel> Login([FromBody] LoginUserDto loginUserDto, CancellationToken cancellationToken)
        {
            var result = await _identityService.LoginUserAsync(loginUserDto, cancellationToken);
            return result;
        }
    }
}
