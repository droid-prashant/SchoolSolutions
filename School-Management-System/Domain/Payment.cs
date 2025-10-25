using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Payment:AuditableEntry
    {
        public Guid Id { get; set; }

        public Guid StudentFeeId { get; set; }
        public StudentFee StudentFee { get; set; } = null!;
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public decimal AmountPaid { get; set; }
        public string Method { get; set; } = "Cash"; // Cash, Khalti, Bank, etc.
        public string? Remarks { get; set; }
    }
}
