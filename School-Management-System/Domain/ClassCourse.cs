using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ClassCourse : AuditableEntry
    {
        public Guid Id { get; set; }
        public Guid ClassRoomId { get; set; }
        public ClassRoom ClassRoom { get; set; }
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
        public bool IsTheoryRequired { get; set; } = true;
        public bool IsPracticalRequired { get; set; } = true;
        public decimal? TheoryCreditHour { get; set; }
        public decimal? PracticalCreditHour { get; set; }
        public decimal? TheoryFullMarks { get; set; }
        public decimal? PracticalFullMarks { get; set; }
        public ICollection<SubjectMark> SubjectMarks { get; set; }

    }
}
