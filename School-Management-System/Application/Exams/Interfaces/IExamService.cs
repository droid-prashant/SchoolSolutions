using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Exams.ViewModels;
using Application.SubjectMarks.Dtos;

namespace Application.SubjectMarks.Interfaces
{
    public interface IExamService
    {
        Task AddSubjectMarks(SubjectMarkDto subjectMarkDto, CancellationToken cancellationToken);
        Task<ResultViewModel> GetResult(Guid studentEnrollmentId, CancellationToken cancellationToken);
    }
}
