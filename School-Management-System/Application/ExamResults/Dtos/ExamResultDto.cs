using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.SubjectMarks.Dtos;

namespace Application.Courses.Dtos
{
    public class ExamResultDto
    {
        public Guid StudentId { get; set; }
        public string ExamType { get; set; }
        public List<SubjectMarkDto> SubjectMarks { get; set; }
    }
}
