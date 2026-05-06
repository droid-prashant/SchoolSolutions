using Domain.Enums;

namespace Application.Attendance.Dtos
{
    public class StudentAttendanceBatchDto
    {
        public string AcademicYearId { get; set; } = string.Empty;
        public string ClassSectionId { get; set; } = string.Empty;
        public DateOnly AttendanceDateEn { get; set; }
        public string AttendanceDateNp { get; set; } = string.Empty;
        public List<StudentAttendanceEntryDto> Entries { get; set; } = new();
    }

    public class StudentAttendanceEntryDto
    {
        public string StudentEnrollmentId { get; set; } = string.Empty;
        public StudentAttendanceStatus Status { get; set; }
        public string? Remarks { get; set; }
    }
}
