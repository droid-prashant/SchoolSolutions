using Application.Dashboard.Interfaces;
using Application.Dashboard.ViewModels;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
        public async Task<int> GetStudentCount(CancellationToken cancellationToken)
        {
            var result = await _dashboardService.GetStudentCount(cancellationToken);
            return result;
        }

        [HttpGet]
        [Route("GetCoursesCount")]
        public async Task<int> GetCoursesCount(CancellationToken cancellationToken)
        {
            var result = await _dashboardService.GetCoursesCount(cancellationToken);
            return result;
        }

        [HttpGet]
        [Route("GetTeachersCount")]
        public async Task<int> GetTeachersCount(CancellationToken cancellationToken)
        {
            return await _dashboardService.GetTeachersCount(cancellationToken);
        }

        [HttpGet]
        [Route("GetDashboardSummary")]
        public async Task<DashboardSummaryViewModel> GetDashboardSummary(CancellationToken cancellationToken)
        {
            return await _dashboardService.GetDashboardSummary(cancellationToken);
        }

        [HttpGet]
        [Route("GetStudentsByClass")]
        public async Task<List<StudentsByClassViewModel>> GetStudentsByClass(CancellationToken cancellationToken)
        {
            var result = await _dashboardService.GetStudentsByClass(cancellationToken);
            return result;
        }
    }
}
