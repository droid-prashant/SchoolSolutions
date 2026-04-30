using Application.Dashboard.Interfaces;
using Application.Dashboard.ViewModels;
using Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class DashboardController : ApiBaseController
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        [Route("GetStudentsCount")]
        [HasPermission(PermissionNames.StudentView)]
        public async Task<int> GetStudentCount(CancellationToken cancellationToken)
        {
            var result = await _dashboardService.GetStudentCount(cancellationToken);
            return result;
        }

        [HttpGet]
        [Route("GetCoursesCount")]
        [HasPermission(PermissionNames.CourseManage, PermissionNames.StudentView)]
        public async Task<int> GetCoursesCount(CancellationToken cancellationToken)
        {
            var result = await _dashboardService.GetCoursesCount(cancellationToken);
            return result;
        }

        [HttpGet]
        [Route("GetTeachersCount")]
        [HasPermission(PermissionNames.TeacherView, PermissionNames.TeacherManage)]
        public async Task<int> GetTeachersCount(CancellationToken cancellationToken)
        {
            return await _dashboardService.GetTeachersCount(cancellationToken);
        }

        [HttpGet]
        [Route("GetDashboardSummary")]
        [HasPermission(PermissionNames.StudentView, PermissionNames.FeeView, PermissionNames.TeacherView)]
        public async Task<DashboardSummaryViewModel> GetDashboardSummary(CancellationToken cancellationToken)
        {
            return await _dashboardService.GetDashboardSummary(cancellationToken);
        }

        [HttpGet]
        [Route("GetStudentsByClass")]
        [HasPermission(PermissionNames.StudentView)]
        public async Task<List<StudentsByClassViewModel>> GetStudentsByClass(CancellationToken cancellationToken)
        {
            var result = await _dashboardService.GetStudentsByClass(cancellationToken);
            return result;
        }
    }
}
