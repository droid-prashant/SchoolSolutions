using Application.Master.Interface;
using Application.Master.ViewModels;
using Application.ClassSections.Interfaces;
using Application.ClassSections.VieModels;
using Application.Courses.Interfaces;
using Application.Courses.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasterDataController : ApiBaseController
    {
        private readonly IMasterDataService _masterDataService;
        private readonly ICourseService _courseService;

        public MasterDataController(
            IMasterDataService masterDataService,
            IClassSectionService classSectionService,
            ICourseService courseService)
        {
            _masterDataService = masterDataService;
            _courseService = courseService;
        }

        [HttpGet]
        [Route("GetAllProvince")]
        public async Task<List<ProvinceViewModel>> GetAllProvince(CancellationToken cancellationToken)
        {
            var result = await _masterDataService.GetAllProvince(cancellationToken);
            return result;
        }

        [HttpGet]
        [Route("GetAllClass")]
        public async Task<List<ClassRoomViewModel>> GetAllClass(CancellationToken cancellationToken)
        {
            var result = await _masterDataService.GetAllClassRooms(cancellationToken);
            return result;
        }

        [HttpGet]
        [Route("GetAllSection")]
        public async Task<List<SectionViewModel>> GetAllSection(CancellationToken cancellationToken)
        {
            var result = await _masterDataService.GetAllSections(cancellationToken);
            return result;
        }

        [HttpGet]
        [Route("GetAllCourses")]
        public async Task<List<CourseViewModel>> GetAllCourses(CancellationToken cancellationToken)
        {
            var result = await _masterDataService.GetAllCourse(cancellationToken);
            return result;
        }
    }
}
