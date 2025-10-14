using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Course : AuditableEntry
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<ClassCourse> ClassCourses { get; set; }
    }
}
