using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Application.Identity.Dtos
{
    public class UserRoleDto
    {
        public string RoleName { get; set; }
        public string Description { get; set; }
        public List<UserPermissionDto> UserPermissions { get; set; } = new List<UserPermissionDto>();
    }
}
