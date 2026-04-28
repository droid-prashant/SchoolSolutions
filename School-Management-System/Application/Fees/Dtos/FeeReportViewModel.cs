using System.Collections.Generic;

namespace Application.Fees.Dtos
{
    public class FeeReportViewModel
    {
        public string AcademicYearName { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public int TotalStudents { get; set; }
        public decimal TotalFees { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalFine { get; set; }
        public decimal NetFees { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalPending { get; set; }
        public decimal TotalPreviousYearPending { get; set; }
        public decimal GrandTotalPending { get; set; }
        public List<FeeReportRowViewModel> Students { get; set; } = new();
    }
}
