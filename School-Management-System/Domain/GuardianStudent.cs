namespace Domain
{
    public class GuardianStudent : AuditableEntry
    {
        public Guid GuardianId { get; set; }
        public Guardian Guardian { get; set; } = null!;
        public Guid StudentId { get; set; }
        public Student Student { get; set; } = null!;
        public bool IsPrimaryGuardian { get; set; }
        public bool CanViewFees { get; set; }
        public bool CanViewResults { get; set; }
        public bool CanViewAttendance { get; set; }
        public bool CanPayFees { get; set; }
    }
}
