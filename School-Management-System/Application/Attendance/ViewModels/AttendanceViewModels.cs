using Domain.Enums;

namespace Application.Attendance.ViewModels
{
    public class AttendanceStatusCountViewModel
    {
        public int Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class AttendanceSummaryViewModel
    {
        public int Total { get; set; }
        public int Present { get; set; }
        public int Absent { get; set; }
        public int Late { get; set; }
        public int Leave { get; set; }
        public int HalfDay { get; set; }
        public int WorkFromHome { get; set; }
        public decimal AttendancePercentage { get; set; }
        public List<AttendanceStatusCountViewModel> StatusCounts { get; set; } = new();
    }

    public class StudentDailyAttendanceViewModel
    {
        public Guid AcademicYearId { get; set; }
        public Guid ClassSectionId { get; set; }
        public DateOnly AttendanceDateEn { get; set; }
        public string AttendanceDateNp { get; set; } = string.Empty;
        public AttendanceSummaryViewModel Summary { get; set; } = new();
        public List<StudentAttendanceRowViewModel> Students { get; set; } = new();
    }

    public class StudentAttendanceRowViewModel
    {
        public Guid? AttendanceId { get; set; }
        public Guid StudentEnrollmentId { get; set; }
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string ClassRoomName { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public int? RollNumber { get; set; }
        public StudentAttendanceStatus Status { get; set; } = StudentAttendanceStatus.Present;
        public string StatusName { get; set; } = nameof(StudentAttendanceStatus.Present);
        public string Remarks { get; set; } = string.Empty;
        public bool HasAttendance { get; set; }
        public bool IsEnrollmentActive { get; set; }
    }

    public class StudentAttendanceReportRowViewModel
    {
        public Guid AttendanceId { get; set; }
        public Guid AcademicYearId { get; set; }
        public Guid ClassSectionId { get; set; }
        public Guid StudentEnrollmentId { get; set; }
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string ClassRoomName { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public int? RollNumber { get; set; }
        public DateOnly AttendanceDateEn { get; set; }
        public string AttendanceDateNp { get; set; } = string.Empty;
        public StudentAttendanceStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public bool IsEnrollmentActive { get; set; }
    }

    public class MonthlyAttendanceReportViewModel<T>
    {
        public Guid AcademicYearId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public AttendanceSummaryViewModel Summary { get; set; } = new();
        public List<T> Rows { get; set; } = new();
    }

    public class StudentAttendanceHistoryViewModel
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public Guid AcademicYearId { get; set; }
        public AttendanceSummaryViewModel Summary { get; set; } = new();
        public List<StudentAttendanceReportRowViewModel> Rows { get; set; } = new();
    }

    public class TeacherDailyAttendanceViewModel
    {
        public Guid AcademicYearId { get; set; }
        public DateOnly AttendanceDateEn { get; set; }
        public string AttendanceDateNp { get; set; } = string.Empty;
        public AttendanceSummaryViewModel Summary { get; set; } = new();
        public List<TeacherAttendanceRowViewModel> Teachers { get; set; } = new();
    }

    public class TeacherAttendanceRowViewModel
    {
        public Guid? AttendanceId { get; set; }
        public Guid TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public TeacherAttendanceStatus Status { get; set; } = TeacherAttendanceStatus.Present;
        public string StatusName { get; set; } = nameof(TeacherAttendanceStatus.Present);
        public TimeOnly? CheckInTime { get; set; }
        public TimeOnly? CheckOutTime { get; set; }
        public string Remarks { get; set; } = string.Empty;
        public bool HasAttendance { get; set; }
        public bool IsTeacherActive { get; set; }
    }

    public class TeacherAttendanceReportRowViewModel
    {
        public Guid AttendanceId { get; set; }
        public Guid AcademicYearId { get; set; }
        public Guid TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public DateOnly AttendanceDateEn { get; set; }
        public string AttendanceDateNp { get; set; } = string.Empty;
        public TeacherAttendanceStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public TimeOnly? CheckInTime { get; set; }
        public TimeOnly? CheckOutTime { get; set; }
        public string Remarks { get; set; } = string.Empty;
        public bool IsTeacherActive { get; set; }
    }
}
