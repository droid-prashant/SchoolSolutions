using Application.Attendance.Dtos;
using Application.Attendance.ViewModels;

namespace Application.Attendance.Interfaces
{
    public interface ITeacherAttendanceService
    {
        Task<TeacherDailyAttendanceViewModel> UpsertDailyAttendanceAsync(TeacherAttendanceBatchDto request, CancellationToken cancellationToken);
        Task<TeacherDailyAttendanceViewModel> GetAttendanceByDateAsync(string academicYearId, DateOnly attendanceDateEn, CancellationToken cancellationToken);
        Task<MonthlyAttendanceReportViewModel<TeacherAttendanceReportRowViewModel>> GetMonthlyAttendanceAsync(string academicYearId, int year, int month, string? teacherId, CancellationToken cancellationToken);
        Task<List<TeacherAttendanceReportRowViewModel>> GetLateArrivalReportAsync(string academicYearId, int year, int month, CancellationToken cancellationToken);
        Task<List<TeacherAttendanceReportRowViewModel>> GetLeaveReportAsync(string academicYearId, int year, int month, CancellationToken cancellationToken);
        Task<AttendanceSummaryViewModel> GetAttendanceSummaryAsync(string academicYearId, DateOnly attendanceDateEn, CancellationToken cancellationToken);
        Task<TeacherAttendanceRowViewModel> CheckInAsync(TeacherCheckInOutDto request, CancellationToken cancellationToken);
        Task<TeacherAttendanceRowViewModel> CheckOutAsync(TeacherCheckInOutDto request, CancellationToken cancellationToken);
        Task DeleteAttendanceAsync(string attendanceId, CancellationToken cancellationToken);
    }
}
