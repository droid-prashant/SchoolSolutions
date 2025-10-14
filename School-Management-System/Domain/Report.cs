using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Report:AuditableEntry
    {
        public Guid Id { get; set; }
        public int AcademicYear { get; set; }
        public string Terminal { get; set; }
        public double GPA { get; set; }
        public double Grade { get; set; }
        public Student Student { get; set; }
        public Guid StudentId { get; set; }
        public ClassRoom ClassRoom { get; set; }
        public Guid ClassId{ get; set; }
    }
}
