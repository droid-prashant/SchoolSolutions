using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class AcademicYear:AuditableEntry
    {
        public string YearName { get; set; } = string.Empty; 
        public string StartDateNp { get; set; }
        public string EndDateNp { get; set; }
        public string StartDateEn { get; set; }
        public string EndDateEn { get; set; }
        public ICollection<StudentEnrollment> Enrollments { get; set; } = new List<StudentEnrollment>();
        public ICollection<TeacherClassSection> TeacherClassSections { get; set; } = new List<TeacherClassSection>();
    }
}
