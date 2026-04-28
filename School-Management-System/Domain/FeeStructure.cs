using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class FeeStructure : AuditableEntry
    {
        public Guid Id { get; set; }
        public Guid AcademicYearId { get; set; }
        public AcademicYear AcademicYear { get; set; }
        public Guid FeeTypeId { get; set; }
        public FeeType FeeType { get; set; }
        public Guid ClassId { get; set; }
        public ClassRoom ClassRoom { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public ICollection<StudentFee> StudentFees { get; set; } = new List<StudentFee>();
    }
}
