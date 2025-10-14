using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class TeacherClassSection : AuditableEntry
    {
        public Guid Id { get; set; }
        public Teacher Teacher { get; set; }
        public Guid TeacherId { get; set; }
        public ClassSection ClassSection { get; set; }
        public Guid ClassSectionId { get; set; }
    }
}
