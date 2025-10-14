using Application.Students.Dtos;
using Application.Students.Interfaces;
using Application.Students.ViewModels;
using Application.SubjectMarks.Dtos;
using Application.SubjectMarks.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamController
    {
        private readonly IExamService _examService;
        public ExamController(IExamService examService)
        {        //[HttpGet]
        //[Route("GetResult")]
        //public async Task<List<StudentViewModel>> Get(CancellationToken cancellationToken)
        //{
        //    var result = await _studentService.GetStudentAsync(cancellationToken);
        //    return result;
        //}

        //[HttpGet()]
        //[Route("GetResultByStudentId")]
        //public async Task<List<StudentViewModel>> GetStudentByClassId([FromQuery] string classId, CancellationToken cancellationToken)
        //{
        //    var classRoomId = Guid.Parse(classId);
        //    var result = await _studentService.GetStudentByClassIdAsync(classRoomId, cancellationToken);
        //    return result;
        //}
            _examService = examService;
        }

        [HttpPost]
        [Route("AddMarks")]
        public async Task Post([FromBody] SubjectMarkDto subjectMarkDto, CancellationToken cancellationToken)
        {
            await _examService.AddSubjectMarks(subjectMarkDto, cancellationToken);
        }

        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
