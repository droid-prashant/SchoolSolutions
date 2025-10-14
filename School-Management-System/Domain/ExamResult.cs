using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ExamResult
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Student Student { get; set; }
        public string ExamType { get; set; }
        public double TotalCredit { get; set; }
        public double GPA { get; set; }
        public ICollection<SubjectMark> SubjectMarks { get; set; }
    }
}
