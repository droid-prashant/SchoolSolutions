using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Fees.Dtos
{
    public class StudentFeeSummaryViewModel
    {
        public Guid StudentEnrollmentId { get; set; }
        public string AcademicYearName { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public decimal TotalFees { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalFine { get; set; }
        public decimal NetFees { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalPending { get; set; }
        public decimal PreviousYearPending { get; set; }
        public decimal GrandTotalPending { get; set; }

        public List<StudentFeeDetailViewModel> FeeDetails { get; set; } = new();
        public List<PreviousYearDueViewModel> PreviousYearDues { get; set; } = new();
        public List<StudentFeeDetailViewModel> PreviousYearFeeDetails { get; set; } = new();
    }
}
