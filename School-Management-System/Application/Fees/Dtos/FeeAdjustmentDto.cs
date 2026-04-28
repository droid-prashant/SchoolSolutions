using System;

namespace Application.Fees.Dtos
{
    public class FeeAdjustmentDto
    {
        public Guid StudentFeeId { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FineAmount { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
