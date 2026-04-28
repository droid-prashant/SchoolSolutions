using System;

namespace Application.Fees.Dtos
{
    public class FeeReportRowViewModel
    {
        public Guid StudentEnrollmentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public int? RollNumber { get; set; }
        public decimal TotalFees { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalFine { get; set; }
        public decimal NetFees { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalPending { get; set; }
        public decimal PreviousYearPending { get; set; }
        public decimal GrandTotalPending { get; set; }
        public bool HasPendingFees => TotalPending > 0;
    }
}
