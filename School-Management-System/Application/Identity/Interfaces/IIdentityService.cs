using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Identity.Dtos;
using Application.Identity.ViewModel;

namespace Application.Identity.Interfaces
{
    public interface IIdentityService
    {
        Task CreateRoleAsync(UserRoleDto userRoleDto, CancellationToken cancellationToken);
        Task CreateUserAsync(UserDto userDto, CancellationToken cancellationToken);
        Task<TokenViewModel> LoginUserAsync(LoginUserDto loginUserDto, CancellationToken cancellationToken);
    }
}
