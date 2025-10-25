using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Fees.Dtos
{
    public class StudentFeeSummaryViewModel
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public decimal TotalFees { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalPending { get; set; }

        public List<StudentFeeDetailViewModel> FeeDetails { get; set; } = new();
    }
}
