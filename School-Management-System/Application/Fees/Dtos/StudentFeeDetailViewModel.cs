using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Fees.Dtos
{
    public class StudentFeeDetailViewModel
    {
        public string ClassName { get; set; } = string.Empty;
        public string FeeType { get; set; } = string.Empty;
        public DateTime? FeeMonth { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal PendingAmount { get; set; }
        public bool IsPaid { get; set; }
    }
}
