using Application.Guardians.Dtos;
using Application.Guardians.Interfaces;
using Application.Guardians.ViewModels;
using Application.Attendance.Interfaces;
using Application.Attendance.ViewModels;
using Application.Exams.Interfaces;
using Application.Exams.ViewModels;
using Application.Fees.Dtos;
using Application.Fees.Interfaces;
using Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuardianController : ControllerBase
    {
        private readonly IGuardianService _guardianService;
        private readonly IFeeService _feeService;
        private readonly IStudentAttendanceService _studentAttendanceService;
        private readonly IExamService _examService;

        public GuardianController(
            IGuardianService guardianService,
            IFeeService feeService,
            IStudentAttendanceService studentAttendanceService,
            IExamService examService)
        {
            _guardianService = guardianService;
            _feeService = feeService;
            _studentAttendanceService = studentAttendanceService;
            _examService = examService;
        }

        [HttpGet("GetStudentGuardians")]
        [HasPermission(PermissionNames.GuardianManage, PermissionNames.StudentUpdate, PermissionNames.UserManage)]
        public async Task<List<StudentGuardianViewModel>> GetStudentGuardians([FromQuery] string studentId, CancellationToken cancellationToken)
        {
            return await _guardianService.GetStudentGuardiansAsync(studentId, cancellationToken);
        }

        [HttpGet("SearchGuardians")]
        [HasPermission(PermissionNames.GuardianManage, PermissionNames.StudentUpdate, PermissionNames.UserManage)]
        public async Task<List<GuardianViewModel>> SearchGuardians([FromQuery] string? keyword, CancellationToken cancellationToken)
        {
            return await _guardianService.SearchGuardiansAsync(keyword, cancellationToken);
        }

        [HttpPost("CreateGuardianForStudent")]
        [HasPermission(PermissionNames.GuardianManage, PermissionNames.StudentUpdate, PermissionNames.UserManage)]
        public async Task<StudentGuardianViewModel> CreateGuardianForStudent([FromQuery] string studentId, [FromBody] GuardianCreateDto guardianCreateDto, CancellationToken cancellationToken)
        {
            return await _guardianService.CreateGuardianForStudentAsync(studentId, guardianCreateDto, cancellationToken);
        }

        [HttpPost("LinkGuardianToStudent")]
        [HasPermission(PermissionNames.GuardianManage, PermissionNames.StudentUpdate, PermissionNames.UserManage)]
        public async Task<StudentGuardianViewModel> LinkGuardianToStudent([FromBody] GuardianStudentLinkDto guardianStudentLinkDto, CancellationToken cancellationToken)
        {
            return await _guardianService.LinkGuardianToStudentAsync(guardianStudentLinkDto, cancellationToken);
        }

        [HttpPut("UpdateStudentGuardianAccess")]
        [HasPermission(PermissionNames.GuardianManage, PermissionNames.StudentUpdate, PermissionNames.UserManage)]
        public async Task UpdateStudentGuardianAccess([FromQuery] string guardianStudentId, [FromBody] GuardianStudentAccessDto guardianStudentAccessDto, CancellationToken cancellationToken)
        {
            await _guardianService.UpdateStudentGuardianAccessAsync(guardianStudentId, guardianStudentAccessDto, cancellationToken);
        }

        [HttpDelete("UnlinkGuardianFromStudent")]
        [HasPermission(PermissionNames.GuardianManage, PermissionNames.StudentUpdate, PermissionNames.UserManage)]
        public async Task UnlinkGuardianFromStudent([FromQuery] string guardianStudentId, CancellationToken cancellationToken)
        {
            await _guardianService.UnlinkGuardianFromStudentAsync(guardianStudentId, cancellationToken);
        }

        [HttpGet("GetCurrentGuardianStudents")]
        [HasPermission(
            PermissionNames.GuardianStudentProfileView,
            PermissionNames.GuardianAttendanceView,
            PermissionNames.GuardianResultView,
            PermissionNames.GuardianFeeView,
            PermissionNames.GuardianFeePay)]
        public async Task<List<GuardianLinkedStudentViewModel>> GetCurrentGuardianStudents(CancellationToken cancellationToken)
        {
            return await _guardianService.GetCurrentGuardianStudentsAsync(cancellationToken);
        }

        [HttpGet("GetCurrentGuardianStudentFeeSummary")]
        [HasPermission(PermissionNames.GuardianFeeView, PermissionNames.GuardianFeePay)]
        public async Task<StudentFeeSummaryViewModel> GetCurrentGuardianStudentFeeSummary([FromQuery] string studentEnrollmentId, [FromQuery] string classSectionId, CancellationToken cancellationToken)
        {
            await _guardianService.EnsureCurrentGuardianCanViewFeesAsync(studentEnrollmentId, cancellationToken);
            return await _feeService.GetStudentFeeSummary(studentEnrollmentId, classSectionId, cancellationToken);
        }

        [HttpGet("GetCurrentGuardianStudentAttendance")]
        [HasPermission(PermissionNames.GuardianAttendanceView)]
        public async Task<StudentAttendanceHistoryViewModel> GetCurrentGuardianStudentAttendance([FromQuery] string studentId, [FromQuery] string academicYearId, CancellationToken cancellationToken)
        {
            await _guardianService.EnsureCurrentGuardianCanViewAttendanceAsync(studentId, cancellationToken);
            return await _studentAttendanceService.GetAttendanceByStudentAsync(studentId, academicYearId, cancellationToken);
        }

        [HttpGet("GetCurrentGuardianStudentResult")]
        [HasPermission(PermissionNames.GuardianResultView)]
        public async Task<ResultViewModel> GetCurrentGuardianStudentResult([FromQuery] string studentEnrollmentId, [FromQuery] int? examType, CancellationToken cancellationToken)
        {
            await _guardianService.EnsureCurrentGuardianCanViewResultsAsync(studentEnrollmentId, cancellationToken);
            return await _examService.GetResult(Guid.Parse(studentEnrollmentId), examType, cancellationToken);
        }
    }
}
