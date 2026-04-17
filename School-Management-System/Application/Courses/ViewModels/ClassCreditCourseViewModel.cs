using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;

namespace Application.Courses.ViewModels
{
    public class ClassCreditCourseViewModel
    {
        public Guid ClassCreditCourseId { get; set; }
        public Guid ClassRoomId { get; set; }
        public Guid CourseId { get; set; }
        public string CourseName { get; set; }
        public string ClassName { get; set; }
        public bool IsTheoryRequired { get; set; }
        public bool IsPracticalRequired { get; set; }
        public decimal? TheoryFullMarks { get; set; }
        public decimal? PracticalFullMarks { get; set; }
        public decimal? TheoryCreditHour { get; set; }
        public decimal? PracticalCreditHour { get; set; }
    }
}
