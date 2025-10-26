using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum Permission
    {
        [Display(GroupName = "User Management", Name = "userMgmt:administrate-user", Description = "Can administrate user")]
        AdministrateUsers = 1,

        [Display(GroupName = "User Management", Name = "userMgmt:view-user", Description = "Can view user")]
        ViewUsers = 2,

        [Display(GroupName = "Role Management", Name = "userMgmt:administrate-role", Description = "Can Administrate Role.")]
        AdministrateRoles = 3,

        [Display(GroupName = "Role Management", Name = "userMgmt:view-role", Description = "Can View Role.")]
        ViewRoles = 4,
    }
}
