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

        [Display(GroupName = "Teacher Management", Name = "teacher:view", Description = "Can view teachers.")]
        ViewTeachers = 20,

        [Display(GroupName = "Teacher Management", Name = "teacher:create", Description = "Can create teachers.")]
        CreateTeachers = 21,

        [Display(GroupName = "Teacher Management", Name = "teacher:update", Description = "Can update teachers.")]
        UpdateTeachers = 22,

        [Display(GroupName = "Teacher Management", Name = "teacher:delete", Description = "Can delete teachers.")]
        DeleteTeachers = 23,

        [Display(GroupName = "Teacher Management", Name = "teacher:assign", Description = "Can assign teachers to classes, sections, and courses.")]
        AssignTeachers = 24,

        [Display(GroupName = "Teacher Management", Name = "teacher:reports", Description = "Can view teacher reports.")]
        ViewTeacherReports = 25,
    }
}
