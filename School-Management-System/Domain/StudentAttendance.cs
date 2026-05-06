using Domain.Enums;

namespace Domain
{
    public class StudentAttendance : AuditableEntry
    {
        public Guid StudentEnrollmentId { get; set; }
        public StudentEnrollment StudentEnrollment { get; set; }
        public Guid StudentId { get; set; }
        public Student Student { get; set; }
        public Guid ClassSectionId { get; set; }
        public ClassSection ClassSection { get; set; }
        public Guid AcademicYearId { get; set; }
        public AcademicYear AcademicYear { get; set; }
        public DateOnly AttendanceDateEn { get; set; }
        public string AttendanceDateNp { get; set; } = string.Empty;
        public StudentAttendanceStatus Status { get; set; }
        public string Remarks { get; set; } = string.Empty;
        public Guid RecordedByUserId { get; set; }
    }
}
