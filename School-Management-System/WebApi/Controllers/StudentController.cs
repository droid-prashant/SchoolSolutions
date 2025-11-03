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
        public async Task AddStudent([FromBody] StudentDto addStudent, CancellationToken cancellationToken)
        {
            await _studentService.AddStudentAsync(addStudent, cancellationToken);
        }

        [HttpPost]
        [Route("AssignRegistrationAndSymbolNumber")]
        public async Task AssignRegistrationAndSymbolNumber([FromBody] StudentEnrollmentDto studentEnrollmentDto, [FromQuery] string studentEnrollmentId, CancellationToken cancellationToken)
        {
            await _studentService.AssignRegistrationAndSymbolNumber(studentEnrollmentDto, studentEnrollmentId, cancellationToken);
        }

        [HttpPost]
        [Route("UpdateStudent")]
        public async Task UpdateStudent([FromBody] StudentDto addStudent, [FromQuery] string studentId, CancellationToken cancellationToken)
        {
            await _studentService.UpdateStudentAsync(addStudent, studentId, cancellationToken);
        }

        [HttpPost]
        [Route("AssignRollNumber")]
        public async Task AssignRollNumbersAsync([FromQuery] string classSectionId, CancellationToken cancellationToken)
        {
            await _studentService.AssignRollNumbersAsync(classSectionId, cancellationToken);
        }

        [HttpGet]
        [Route("GetRegAndSymCompliantEnrolledStudents")]
        public async Task<List<StudentEnrollmentViewModel>> GetRegAndSymCompliantEnrolledStudents([FromQuery] string classSectionId, CancellationToken cancellationToken)
        {
            var result = await _studentService.GetRegAndSymCompliantEnrolledStudents(classSectionId, cancellationToken);
            return result;
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
