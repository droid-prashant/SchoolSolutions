using Application.Exams.ViewModels;
using Application.SubjectMarks.Dtos;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.SubjectMarks.Interfaces
{
    public interface IExamService
    {
        Task AddSubjectMarks(SubjectMarkDto subjectMarkDto, CancellationToken cancellationToken);
        Task UpdateSubjectMarks(SubjectMarkDto subjectMarkDto, CancellationToken cancellationToken);
        Task UpsertTeacherSubjectMarks(SubjectMarkDto subjectMarkDto, CancellationToken cancellationToken);
        Task DeleteTeacherSubjectMarks(string studentEnrollmentId, int examType, string classCourseId, CancellationToken cancellationToken);
        Task<SubjectMarksViewModel> GetStudentMarks(string studentEnrollmentId, int examType, CancellationToken cancellationToken);
        Task<SubjectMarksViewModel?> GetTeacherStudentSubjectMarks(string studentEnrollmentId, int examType, string classCourseId, CancellationToken cancellationToken);
        Task<List<TeacherMarksAssignmentViewModel>> GetTeacherMarksAssignments(CancellationToken cancellationToken);
        Task<List<TeacherSubjectStudentMarksViewModel>> GetTeacherSubjectStudentMarks(string classSectionId, string classCourseId, int examType, string? keyword, CancellationToken cancellationToken);
        Task<ResultViewModel> GetResult(Guid studentEnrollmentId, int? examType, CancellationToken cancellationToken);
    }
}
