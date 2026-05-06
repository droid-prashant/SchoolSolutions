using Application.Attendance.Dtos;
using Application.Attendance.Interfaces;
using Application.Attendance.ViewModels;
using Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherAttendanceController : ApiBaseController
    {
        private readonly ITeacherAttendanceService _teacherAttendanceService;

        public TeacherAttendanceController(ITeacherAttendanceService teacherAttendanceService)
        {
            _teacherAttendanceService = teacherAttendanceService;
        }

        [HttpPost("UpsertDailyAttendance")]
        [HasPermission(PermissionNames.TeacherAttendanceTake, PermissionNames.TeacherAttendanceEdit)]
        public async Task<TeacherDailyAttendanceViewModel> UpsertDailyAttendance([FromBody] TeacherAttendanceBatchDto request, CancellationToken cancellationToken)
        {
            return await _teacherAttendanceService.UpsertDailyAttendanceAsync(request, cancellationToken);
        }

        [HttpGet("GetAttendanceByDate")]
        [HasPermission(PermissionNames.TeacherAttendanceView, PermissionNames.TeacherAttendanceTake, PermissionNames.TeacherAttendanceEdit)]
        public async Task<TeacherDailyAttendanceViewModel> GetAttendanceByDate([FromQuery] string academicYearId, [FromQuery] DateOnly attendanceDateEn, CancellationToken cancellationToken)
        {
            return await _teacherAttendanceService.GetAttendanceByDateAsync(academicYearId, attendanceDateEn, cancellationToken);
        }

        [HttpGet("GetMonthlyAttendance")]
        [HasPermission(PermissionNames.TeacherAttendanceReport)]
        public async Task<MonthlyAttendanceReportViewModel<TeacherAttendanceReportRowViewModel>> GetMonthlyAttendance([FromQuery] string academicYearId, [FromQuery] int year, [FromQuery] int month, [FromQuery] string? teacherId, CancellationToken cancellationToken)
        {
            return await _teacherAttendanceService.GetMonthlyAttendanceAsync(academicYearId, year, month, teacherId, cancellationToken);
        }

        [HttpGet("GetLateArrivalReport")]
        [HasPermission(PermissionNames.TeacherAttendanceReport)]
        public async Task<List<TeacherAttendanceReportRowViewModel>> GetLateArrivalReport([FromQuery] string academicYearId, [FromQuery] int year, [FromQuery] int month, CancellationToken cancellationToken)
        {
            return await _teacherAttendanceService.GetLateArrivalReportAsync(academicYearId, year, month, cancellationToken);
        }

        [HttpGet("GetLeaveReport")]
        [HasPermission(PermissionNames.TeacherAttendanceReport)]
        public async Task<List<TeacherAttendanceReportRowViewModel>> GetLeaveReport([FromQuery] string academicYearId, [FromQuery] int year, [FromQuery] int month, CancellationToken cancellationToken)
        {
            return await _teacherAttendanceService.GetLeaveReportAsync(academicYearId, year, month, cancellationToken);
        }

        [HttpGet("GetAttendanceSummary")]
        [HasPermission(PermissionNames.TeacherAttendanceView, PermissionNames.TeacherAttendanceReport)]
        public async Task<AttendanceSummaryViewModel> GetAttendanceSummary([FromQuery] string academicYearId, [FromQuery] DateOnly attendanceDateEn, CancellationToken cancellationToken)
        {
            return await _teacherAttendanceService.GetAttendanceSummaryAsync(academicYearId, attendanceDateEn, cancellationToken);
        }

        [HttpPost("CheckIn")]
        [HasPermission(PermissionNames.TeacherAttendanceCheckInOut)]
        public async Task<TeacherAttendanceRowViewModel> CheckIn([FromBody] TeacherCheckInOutDto request, CancellationToken cancellationToken)
        {
            return await _teacherAttendanceService.CheckInAsync(request, cancellationToken);
        }

        [HttpPost("CheckOut")]
        [HasPermission(PermissionNames.TeacherAttendanceCheckInOut)]
        public async Task<TeacherAttendanceRowViewModel> CheckOut([FromBody] TeacherCheckInOutDto request, CancellationToken cancellationToken)
        {
            return await _teacherAttendanceService.CheckOutAsync(request, cancellationToken);
        }

        [HttpDelete("DeleteAttendance")]
        [HasPermission(PermissionNames.TeacherAttendanceDelete)]
        public async Task DeleteAttendance([FromQuery] string attendanceId, CancellationToken cancellationToken)
        {
            await _teacherAttendanceService.DeleteAttendanceAsync(attendanceId, cancellationToken);
        }
    }
}
