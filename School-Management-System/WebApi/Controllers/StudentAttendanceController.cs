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
    public class StudentAttendanceController : ApiBaseController
    {
        private readonly IStudentAttendanceService _studentAttendanceService;

        public StudentAttendanceController(IStudentAttendanceService studentAttendanceService)
        {
            _studentAttendanceService = studentAttendanceService;
        }

        [HttpPost("UpsertDailyAttendance")]
        [HasPermission(PermissionNames.StudentAttendanceTake, PermissionNames.StudentAttendanceEdit)]
        public async Task<StudentDailyAttendanceViewModel> UpsertDailyAttendance([FromBody] StudentAttendanceBatchDto request, CancellationToken cancellationToken)
        {
            return await _studentAttendanceService.UpsertDailyAttendanceAsync(request, cancellationToken);
        }

        [HttpGet("GetAttendanceByDate")]
        [HasPermission(PermissionNames.StudentAttendanceView, PermissionNames.StudentAttendanceTake, PermissionNames.StudentAttendanceEdit)]
        public async Task<StudentDailyAttendanceViewModel> GetAttendanceByDate([FromQuery] string academicYearId, [FromQuery] string classSectionId, [FromQuery] DateOnly attendanceDateEn, CancellationToken cancellationToken)
        {
            return await _studentAttendanceService.GetAttendanceByDateAsync(academicYearId, classSectionId, attendanceDateEn, cancellationToken);
        }

        [HttpGet("GetAttendanceByStudent")]
        [HasPermission(PermissionNames.StudentAttendanceReport)]
        public async Task<StudentAttendanceHistoryViewModel> GetAttendanceByStudent([FromQuery] string studentId, [FromQuery] string academicYearId, CancellationToken cancellationToken)
        {
            return await _studentAttendanceService.GetAttendanceByStudentAsync(studentId, academicYearId, cancellationToken);
        }

        [HttpGet("GetMonthlyAttendance")]
        [HasPermission(PermissionNames.StudentAttendanceReport)]
        public async Task<MonthlyAttendanceReportViewModel<StudentAttendanceReportRowViewModel>> GetMonthlyAttendance([FromQuery] string academicYearId, [FromQuery] string classSectionId, [FromQuery] int year, [FromQuery] int month, CancellationToken cancellationToken)
        {
            return await _studentAttendanceService.GetMonthlyAttendanceAsync(academicYearId, classSectionId, year, month, cancellationToken);
        }

        [HttpGet("GetAbsentStudents")]
        [HasPermission(PermissionNames.StudentAttendanceReport)]
        public async Task<List<StudentAttendanceReportRowViewModel>> GetAbsentStudents([FromQuery] string academicYearId, [FromQuery] string classSectionId, [FromQuery] DateOnly attendanceDateEn, CancellationToken cancellationToken)
        {
            return await _studentAttendanceService.GetAbsentStudentsAsync(academicYearId, classSectionId, attendanceDateEn, cancellationToken);
        }

        [HttpGet("GetAttendanceSummary")]
        [HasPermission(PermissionNames.StudentAttendanceView, PermissionNames.StudentAttendanceReport)]
        public async Task<AttendanceSummaryViewModel> GetAttendanceSummary([FromQuery] string academicYearId, [FromQuery] string classSectionId, [FromQuery] DateOnly attendanceDateEn, CancellationToken cancellationToken)
        {
            return await _studentAttendanceService.GetAttendanceSummaryAsync(academicYearId, classSectionId, attendanceDateEn, cancellationToken);
        }

        [HttpDelete("DeleteAttendance")]
        [HasPermission(PermissionNames.StudentAttendanceDelete)]
        public async Task DeleteAttendance([FromQuery] string attendanceId, CancellationToken cancellationToken)
        {
            await _studentAttendanceService.DeleteAttendanceAsync(attendanceId, cancellationToken);
        }
    }
}
