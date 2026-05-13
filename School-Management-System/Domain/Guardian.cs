namespace Domain
{
    public class Guardian : AuditableEntry
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string RelationType { get; set; } = string.Empty;
        public ICollection<GuardianStudent> GuardianStudents { get; set; } = new List<GuardianStudent>();
    }
}
