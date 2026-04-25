namespace Domain
{
    public class TeacherExperience : AuditableEntry
    {
        public Guid TeacherId { get; set; }
        public Teacher Teacher { get; set; }
        public string OrganizationName { get; set; }
        public string Designation { get; set; }
        public string SubjectOrDepartment { get; set; }
        public string StartDateNp { get; set; }
        public string StartDateEn { get; set; }
        public string EndDateNp { get; set; }
        public string EndDateEn { get; set; }
        public bool IsCurrent { get; set; }
        public string Remarks { get; set; }
    }
}
