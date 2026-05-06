using Domain.Enums;

namespace Domain
{
    public class TeacherAttendance : AuditableEntry
    {
        public Guid TeacherId { get; set; }
        public Teacher Teacher { get; set; }
        public Guid AcademicYearId { get; set; }
        public AcademicYear AcademicYear { get; set; }
        public DateOnly AttendanceDateEn { get; set; }
        public string AttendanceDateNp { get; set; } = string.Empty;
        public TeacherAttendanceStatus Status { get; set; }
        public TimeOnly? CheckInTime { get; set; }
        public TimeOnly? CheckOutTime { get; set; }
        public string Remarks { get; set; } = string.Empty;
        public Guid RecordedByUserId { get; set; }
    }
}
