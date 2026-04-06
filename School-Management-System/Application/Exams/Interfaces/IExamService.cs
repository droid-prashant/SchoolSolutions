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
        Task<SubjectMarksViewModel> GetStudentMarks(string studentEnrollmentId, int examType, CancellationToken cancellationToken);
        Task<ResultViewModel> GetResult(Guid studentEnrollmentId, CancellationToken cancellationToken);
    }
}
