using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class FeeAdjustment : AuditableEntry
    {
        public Guid Id { get; set; }

        public Guid StudentFeeId { get; set; }
        public StudentFee StudentFee { get; set; } = null!;

        public decimal DiscountAmount { get; set; } = 0;
        public decimal FineAmount { get; set; } = 0;
        public string? Reason { get; set; }
    }
}
