using Application.Attendance.Dtos;
using Application.Attendance.ViewModels;

namespace Application.Attendance.Interfaces
{
    public interface IStudentAttendanceService
    {
        Task<StudentDailyAttendanceViewModel> UpsertDailyAttendanceAsync(StudentAttendanceBatchDto request, CancellationToken cancellationToken);
        Task<StudentDailyAttendanceViewModel> GetAttendanceByDateAsync(string academicYearId, string classSectionId, DateOnly attendanceDateEn, CancellationToken cancellationToken);
        Task<StudentAttendanceHistoryViewModel> GetAttendanceByStudentAsync(string studentId, string academicYearId, CancellationToken cancellationToken);
        Task<MonthlyAttendanceReportViewModel<StudentAttendanceReportRowViewModel>> GetMonthlyAttendanceAsync(string academicYearId, string classSectionId, int year, int month, CancellationToken cancellationToken);
        Task<List<StudentAttendanceReportRowViewModel>> GetAbsentStudentsAsync(string academicYearId, string classSectionId, DateOnly attendanceDateEn, CancellationToken cancellationToken);
        Task<AttendanceSummaryViewModel> GetAttendanceSummaryAsync(string academicYearId, string classSectionId, DateOnly attendanceDateEn, CancellationToken cancellationToken);
        Task DeleteAttendanceAsync(string attendanceId, CancellationToken cancellationToken);
    }
}
