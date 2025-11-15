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
        public string ExamType { get; set; }
        public double TotalCredit { get; set; }
        public double GPA { get; set; }
        public int Attendance { get; set; }
        public int TotalSchoolDays { get; set; }
        public ICollection<SubjectMark> SubjectMarks { get; set; }
    }
}
