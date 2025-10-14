using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;

namespace Application.Courses.Dtos
{
    public class ClassCourseDto
    {
        public string? ClassCourseId { get; set; }
        public string ClassRoomId { get; set; }
        public string CourseId { get; set; }
        public decimal TheoryCreditHour { get; set; }
        public decimal PracticalCreditHour { get; set; }
        public decimal TheoryFullMarks { get; set; }
        public decimal PracticalFullMarks { get; set; }
    }
}
