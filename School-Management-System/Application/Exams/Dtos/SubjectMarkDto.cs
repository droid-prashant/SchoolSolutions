using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.SubjectMarks.Dtos
{
    public class SubjectMarkDto
    {
        public string StudentId { get; set; }
        public required int ExamType { get; set; }
        public int Attendance { get; set; }
        public int TotalSchoolDays { get; set; }
        public required List<StudentMarksList> StudentMarksLists { get; set; }
    }
    public class StudentMarksList
    {
        public string ClassCourseId { get; set; }
        public decimal TheoryCredit { get; set; }
        public decimal PracticalCredit { get; set; }
        public decimal TheoryFullMarks { get; set; }
        public decimal PracticalFullMarks { get; set; }
        public decimal ObtainedTheoryMarks { get; set; }
        public decimal ObtainedPracticalMarks { get; set; }
    }
}

