namespace Application.Fees.ViewModel
{
    public class BulkManualChargeResultViewModel
    {
        public int RequestedCount { get; set; }
        public int EligibleCount { get; set; }
        public int AssignedCount { get; set; }
        public int AlreadyAssignedCount { get; set; }
        public int InvalidEnrollmentCount { get; set; }
    }
}
