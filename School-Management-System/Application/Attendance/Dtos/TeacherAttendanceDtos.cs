using Domain.Enums;

namespace Application.Attendance.Dtos
{
    public class TeacherAttendanceBatchDto
    {
        public string AcademicYearId { get; set; } = string.Empty;
        public DateOnly AttendanceDateEn { get; set; }
        public string AttendanceDateNp { get; set; } = string.Empty;
        public List<TeacherAttendanceEntryDto> Entries { get; set; } = new();
    }

    public class TeacherAttendanceEntryDto
    {
        public string TeacherId { get; set; } = string.Empty;
        public TeacherAttendanceStatus Status { get; set; }
        public TimeOnly? CheckInTime { get; set; }
        public TimeOnly? CheckOutTime { get; set; }
        public string? Remarks { get; set; }
    }

    public class TeacherCheckInOutDto
    {
        public string AcademicYearId { get; set; } = string.Empty;
        public DateOnly AttendanceDateEn { get; set; }
        public string AttendanceDateNp { get; set; } = string.Empty;
        public TimeOnly AttendanceTime { get; set; }
        public string? Remarks { get; set; }
    }
}
