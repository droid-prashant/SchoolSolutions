using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class StudentFee:AuditableEntry
    {
        public Guid Id { get; set; }

        public Guid StudentEnrollmentId { get; set; }
        public StudentEnrollment StudentEnrollment { get; set; } = null!;

        public Guid FeeStructureId { get; set; }
        public FeeStructure FeeStructure { get; set; } = null!;

        public DateTime? FeeMonth { get; set; } 
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; } = false;

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<FeeAdjustment> FeeAdjustments { get; set; } = new List<FeeAdjustment>();
    }
}
