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

        [HttpPost]
        [Route("UpsertTeacherSubjectMarks")]
        public async Task UpsertTeacherSubjectMarks([FromBody] SubjectMarkDto subjectMarkDto, CancellationToken cancellationToken)
        {
            await _examService.UpsertTeacherSubjectMarks(subjectMarkDto, cancellationToken);
        }

        [HttpDelete]
        [Route("DeleteTeacherSubjectMarks")]
        public async Task DeleteTeacherSubjectMarks([FromQuery] string studentEnrollmentId, [FromQuery] int examType, [FromQuery] string classCourseId, CancellationToken cancellationToken)
        {
            await _examService.DeleteTeacherSubjectMarks(studentEnrollmentId, examType, classCourseId, cancellationToken);
        }

        [HttpGet]
        [Route("GetStudentMarks")]
        public async Task<SubjectMarksViewModel> GetStudentMarks([FromQuery] string studentEnrollmentId, [FromQuery] int examType, CancellationToken cancellationToken)
        {
            var result = await _examService.GetStudentMarks(studentEnrollmentId, examType, cancellationToken);
            return result;
        }

        [HttpGet]
        [Route("GetTeacherStudentSubjectMarks")]
        public async Task<SubjectMarksViewModel?> GetTeacherStudentSubjectMarks([FromQuery] string studentEnrollmentId, [FromQuery] int examType, [FromQuery] string classCourseId, CancellationToken cancellationToken)
        {
            return await _examService.GetTeacherStudentSubjectMarks(studentEnrollmentId, examType, classCourseId, cancellationToken);
        }

        [HttpGet]
        [Route("GetTeacherMarksAssignments")]
        public async Task<List<TeacherMarksAssignmentViewModel>> GetTeacherMarksAssignments(CancellationToken cancellationToken)
        {
            return await _examService.GetTeacherMarksAssignments(cancellationToken);
        }

        [HttpGet]
        [Route("GetTeacherSubjectStudentMarks")]
        public async Task<List<TeacherSubjectStudentMarksViewModel>> GetTeacherSubjectStudentMarks([FromQuery] string classSectionId, [FromQuery] string classCourseId, [FromQuery] int examType, [FromQuery] string? keyword, CancellationToken cancellationToken)
        {
            return await _examService.GetTeacherSubjectStudentMarks(classSectionId, classCourseId, examType, keyword, cancellationToken);
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
