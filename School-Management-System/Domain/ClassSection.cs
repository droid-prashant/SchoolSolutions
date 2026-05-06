using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ClassSection
    {
        public Guid Id { get; set; }
        public Guid ClassId { get; set; }
        public ClassRoom ClassRoom { get; set; }
        public Guid SectionId { get; set; }
        public Section Section { get; set; }
        public ICollection<Student> Students { get; set; } = new List<Student>();
        public ICollection<TeacherClassSection> TeacherClassSections { get; set; } = new List<TeacherClassSection>();
        public ICollection<StudentEnrollment> StudentEnrollments { get; set; } = new List<StudentEnrollment>();
        public ICollection<StudentAttendance> StudentAttendances { get; set; } = new List<StudentAttendance>();
    }
}
