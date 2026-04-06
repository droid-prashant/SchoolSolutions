using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ExamResult:AuditableEntry
    {
        public Guid Id { get; set; }
        public Guid StudentEnrollmentId { get; set; }
        public StudentEnrollment StudentEnrollment { get; set; }
        public int ExamType { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal GPA { get; set; }
        public int Attendance { get; set; }
        public int TotalSchoolDays { get; set; }
        public ICollection<SubjectMark> SubjectMarks { get; set; }
    }
}
