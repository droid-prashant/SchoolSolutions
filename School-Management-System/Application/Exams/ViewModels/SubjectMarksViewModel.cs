using Application.Exams.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exams.ViewModels
{
    public class SubjectMarksViewModel
    {
        public string StudentId { get; set; }
        public int ExamType { get; set; }
        public int Attendance { get; set; }
        public int TotalSchoolDays { get; set; }
        public required List<StudentMarksList> StudentMarksLists { get; set; }
    }
}
