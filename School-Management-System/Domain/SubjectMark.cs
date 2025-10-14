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
        public double FullTheoryMarks { get; set; }
        public double FullPracticalMarks { get; set; }
        public double ObtainedTheoryMarks { get; set; }
        public double ObtainedPracticalMarks { get; set; }
        public string GradeTheory { get; set; }
        public double GradePointTheory { get; set; }
        public string GradePractical{ get; set; }
        public double GradePointPractical { get; set; }
        public string FinalGrade { get; set; }
        public Guid StudentId { get; set; }
        public Student Student { get; set; }
        public Guid ClassCourseId { get; set; }
        public ClassCourse ClassCourse { get; set; }
        public Guid ExamResultId { get; set; }
        public ExamResult ExamResult { get; set; }
    }
}
