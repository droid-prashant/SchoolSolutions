using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class SubjectMark:AuditableEntry
    {
        public Guid Id { get; set; }
        public decimal FullTheoryMarks { get; set; }
        public decimal FullPracticalMarks { get; set; }
        public decimal ObtainedTheoryMarks { get; set; }
        public decimal ObtainedPracticalMarks { get; set; }
        public string GradeTheory { get; set; }
        public decimal GradePointTheory { get; set; }
        public string GradePractical{ get; set; }
        public decimal GradePointPractical { get; set; }
        public string FinalGrade { get; set; }
        public decimal FinalGradePoint { get; set; }
        public Guid StudentEnrollmentId { get; set; }
        public StudentEnrollment StudentEnrollment { get; set; }
        public Guid ClassCourseId { get; set; }
        public ClassCourse ClassCourse { get; set; }
        public Guid ExamResultId { get; set; }
        public ExamResult ExamResult { get; set; }
    }
}
