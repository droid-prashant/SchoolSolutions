using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.SubjectMarks.Dtos;

namespace Application.SubjectMarks.Interfaces
{
    public interface IExamService
    {
        Task AddSubjectMarks(SubjectMarkDto subjectMarkDto, CancellationToken cancellationToken);
    }
}
