using Application.Students.Dtos;
using Application.Students.Interfaces;
using Application.Students.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController
    {
        private readonly IStudentService _studentService;
        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        [Route("GetStudent")]
        public async Task<List<StudentViewModel>> Get(CancellationToken cancellationToken)
        {
            var result = await _studentService.GetStudentAsync(cancellationToken);
            return result;
        }

        [HttpGet()]
        [Route("GetStudentByClassId")]
        public async Task<List<StudentViewModel>> GetStudentByClassId([FromQuery] string classId, CancellationToken cancellationToken)
        {
            var classRoomId = Guid.Parse(classId);
            var result = await _studentService.GetStudentByClassIdAsync(classRoomId, cancellationToken);
            return result;
        }

        [HttpGet()]
        [Route("GetStudentByClassSectionId")]
        public async Task<List<StudentViewModel>> GetStudentByClassSectionId([FromQuery] string classSectionId, CancellationToken cancellationToken)
        {
            var result = await _studentService.GetStudentByClassSectionId(classSectionId, cancellationToken);
            return result;
        }

        [HttpPost]
        [Route("AddStudent")]
        public async Task Post([FromBody] StudentDto addStudent, CancellationToken cancellationToken)
        {
            await _studentService.AddStudentAsync(addStudent, cancellationToken);
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
