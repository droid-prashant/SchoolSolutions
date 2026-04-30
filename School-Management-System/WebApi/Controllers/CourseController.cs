using System.Reflection.Metadata.Ecma335;
using Application.Courses.Dtos;
using Application.Courses.Interfaces;
using Application.Courses.ViewModels;
using Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;

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
        [Route("GetAllClassCourse")]
        [HasPermission(PermissionNames.CourseManage, PermissionNames.ExamMarksEntry, PermissionNames.TeacherManage)]
        public async Task<List<ClassCreditCourseViewModel>> GetAllClassCourse(CancellationToken cancellationToken)
        {
            var result = await _courseService.GetAllClassCourse(cancellationToken);
            return result;
        }

        [HttpGet]
        [Route("GetClassCourseByClassId")]
        [HasPermission(PermissionNames.CourseManage, PermissionNames.ExamMarksEntry, PermissionNames.TeacherManage)]
        public async Task<List<ClassCreditCourseViewModel>> GetClassCourseByClassId([FromQuery] string classId, CancellationToken cancellationToken)
        {
            var ClassRoomId = Guid.Parse(classId);
            var result = await _courseService.GetClassCourseByClassId(ClassRoomId, cancellationToken);
            return result;
        }

        [HttpPost]
        [Route("AddClassCourse")]
        [HasPermission(PermissionNames.CourseManage)]
        public async Task AddClassCourse([FromBody] ClassCourseDto classCourseDto, CancellationToken cancellationToken)
        {
            await _courseService.AddClassCourse(classCourseDto, cancellationToken);
        } 
        
        [HttpPost]
        [Route("UpdateClassCourse")]
        [HasPermission(PermissionNames.CourseManage)]
        public async Task UpdateClassCourse([FromBody] ClassCourseDto classCourseDto, CancellationToken cancellationToken)
        {
            await _courseService.UpdateClassCourse(classCourseDto, cancellationToken);
        }

        [HttpDelete]
        [Route("DeleteClassCourse/{classCourseId}")]
        [HasPermission(PermissionNames.CourseManage)]
        public async Task DeleteClassCourse ([FromQuery] string classCourseId, CancellationToken cancellationToken)
        {
            await _courseService.DeleteClassCourse(classCourseId, cancellationToken);
        }


        [HttpPost]
        [Route("AddCourse")]
        [HasPermission(PermissionNames.CourseManage)]
        public async Task AddCourse([FromBody] CourseDto courseDto, CancellationToken cancellationToken)
        {
            await _courseService.AddCourse(courseDto, cancellationToken);
        }

        [HttpPut]
        [Route("UpdateCourse")]
        [HasPermission(PermissionNames.CourseManage)]
        public async Task UpdateCourse([FromBody] CourseDto courseDto, CancellationToken cancellationToken)
        {
            await _courseService.UpdateCourse(courseDto, cancellationToken);
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
