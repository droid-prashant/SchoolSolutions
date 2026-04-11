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
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ICollection<StudentEnrollment> Enrollments { get; set; } = new List<StudentEnrollment>();
    }
}
