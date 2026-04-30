namespace Application.Identity.Dtos
{
    public class RoleDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> PermissionCodes { get; set; } = new();
    }

    public class RolePermissionsDto
    {
        public List<string> PermissionCodes { get; set; } = new();
    }

    public class UserRolesDto
    {
        public List<string> RoleNames { get; set; } = new();
    }
}
