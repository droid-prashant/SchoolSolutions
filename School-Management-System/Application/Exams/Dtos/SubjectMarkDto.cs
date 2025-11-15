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
        public required string ExamType { get; set; }
        public int Attendance { get; set; }
        public int TotalSchoolDays { get; set; }
        public required List<StudentMarksList> StudentMarksLists { get; set; }
    }
    public class StudentMarksList
    {
        public string ClassCourseId { get; set; }
        public double TheoryCredit { get; set; }
        public double PracticalCredit { get; set; }
        public double TheoryFullMarks { get; set; }
        public double PracticalFullMarks { get; set; }
        public double ObtainedTheoryMarks { get; set; }
        public double ObtainedPracticalMarks { get; set; }
    }
}

