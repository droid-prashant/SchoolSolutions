using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ClassRoom:AuditableEntry
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string RoomNumber { get; set; }
        public string AcademicYear { get; set; }
        public ICollection<ClassSection> ClassSections { get; set; }
        public ICollection<ClassCourse> ClassCourses { get; set; }
        public ICollection<Course> Courses { get; set; }
    }
}
