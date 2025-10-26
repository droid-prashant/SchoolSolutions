using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class AcademicYear:AuditableEntry
    {
        public Guid Id { get; set; }
        public string YearName { get; set; } = string.Empty; 
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ICollection<StudentEnrollment> Enrollments { get; set; } = new List<StudentEnrollment>();
    }
}
