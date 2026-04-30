namespace Domain
{
    public class RolePermission : AuditableEntry
    {
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }
        public Permission Permission { get; set; } = null!;
    }
}
