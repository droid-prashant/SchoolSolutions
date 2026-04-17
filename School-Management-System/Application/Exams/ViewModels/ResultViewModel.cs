using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Students.ViewModels;

namespace Application.Exams.ViewModels
{
    public class ResultViewModel
    {
        public int ExamType { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal GPA { get; set; }
        public string StudentName { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string DateOfBirth { get; set; }
        public int WardNo { get; set; }
        public int RollNo { get; set; }
        public int Attendance { get; set; }
        public int TotalSchoolDays { get; set; }
        public string ClassRoom { get; set; }
        public string Section { get; set; }
        public DateTime IssueDate { get; set; }
        public List<StudentMarksViewModel> StudentMarks { get; set; }
    }

    public class StudentMarksViewModel
    {
        public string CourseName { get; set; }
        public decimal? TheoryCreditHour { get; set; }
        public decimal? PracticalCreditHour { get; set; }
        public string GradeTheory { get; set; }
        public string GradePractical { get; set; }
        public string FinalGrade { get; set; }
        public decimal FinalGradePoint { get; set; }
    }
}
