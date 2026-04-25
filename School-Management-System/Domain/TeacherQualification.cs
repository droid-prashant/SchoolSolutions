namespace Domain
{
    public class TeacherQualification : AuditableEntry
    {
        public Guid TeacherId { get; set; }
        public Teacher Teacher { get; set; }
        public string DegreeName { get; set; }
        public string InstitutionName { get; set; }
        public string BoardOrUniversity { get; set; }
        public string PassedYear { get; set; }
        public string GradeOrPercentage { get; set; }
        public string MajorSubject { get; set; }
        public string Remarks { get; set; }
    }
}
