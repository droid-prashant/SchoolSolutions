using System.Reflection.Metadata.Ecma335;
using Application.Courses.Dtos;
using Application.Courses.Interfaces;
using Application.Courses.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;
        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }
        [HttpGet]
        [Route("GetCourse")]
        public async Task<List<CourseViewModel>> Get(CancellationToken cancellationToken)
        {
            var result = await _courseService.GetCourse(cancellationToken);
            return result;
        }

        [HttpGet]
        [Route("GetAllClassCourse")]
        public async Task<List<ClassCreditCourseViewModel>> GetAllClassCourse(CancellationToken cancellationToken)
        {
            var result = await _courseService.GetAllClassCourse(cancellationToken);
            return result;
        }

        [HttpGet]
        [Route("GetClassCourseByClassId")]
        public async Task<List<ClassCreditCourseViewModel>> GetClassCourseByClassId([FromQuery] string classId, CancellationToken cancellationToken)
        {
            var ClassRoomId = Guid.Parse(classId);
            var result = await _courseService.GetClassCourseByClassId(ClassRoomId, cancellationToken);
            return result;
        }

        [HttpPost]
        [Route("AddClassCourse")]
        public async Task AddClassCourse([FromBody] ClassCourseDto classCourseDto, CancellationToken cancellationToken)
        {
            await _courseService.AddClassCourse(classCourseDto, cancellationToken);
        } 
        
        [HttpPost]
        [Route("UpdateClassCourse")]
        public async Task UpdateClassCourse([FromBody] ClassCourseDto classCourseDto, CancellationToken cancellationToken)
        {
            await _courseService.UpdateClassCourse(classCourseDto, cancellationToken);
        }

        [HttpPost]
        [Route("AddCourse")]
        public async Task Post([FromBody] CourseDto courseDto, CancellationToken cancellationToken)
        {
            await _courseService.AddCourse(courseDto, cancellationToken);
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
