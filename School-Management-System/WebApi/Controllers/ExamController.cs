using Application.Exams.ViewModels;
using Application.Students.Dtos;
using Application.Students.Interfaces;
using Application.Students.ViewModels;
using Application.SubjectMarks.Dtos;
using Application.SubjectMarks.Interfaces;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamController
    {
        private readonly IExamService _examService;
        public ExamController(IExamService examService)
        {
            _examService = examService;
        }

        [HttpGet()]
        [Route("GetResult")]
        public async Task<ResultViewModel> GetResult([FromQuery] string studentEnrollmentId, [FromQuery] int? examType, CancellationToken cancellationToken)
        {
            var studentEnrollmentIdGuid = Guid.Parse(studentEnrollmentId);
            var result = await _examService.GetResult(studentEnrollmentIdGuid, examType, cancellationToken);
            return result;
        }

        [HttpPost]
        [Route("AddMarks")]
        public async Task Post([FromBody] SubjectMarkDto subjectMarkDto, CancellationToken cancellationToken)
        {
            await _examService.AddSubjectMarks(subjectMarkDto, cancellationToken);
        }

        [HttpPut]
        [Route("UpdateMarks")]
        public async Task UpdateSubjectMarks([FromBody] SubjectMarkDto subjectMarkDto, CancellationToken cancellationToken)
        {
            await _examService.UpdateSubjectMarks(subjectMarkDto, cancellationToken);
        }

        [HttpGet]
        [Route("GetStudentMarks")]
        public async Task<SubjectMarksViewModel> GetStudentMarks([FromQuery] string studentEnrollmentId, [FromQuery] int examType, CancellationToken cancellationToken)
        {
            var result = await _examService.GetStudentMarks(studentEnrollmentId, examType, cancellationToken);
            return result;
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
