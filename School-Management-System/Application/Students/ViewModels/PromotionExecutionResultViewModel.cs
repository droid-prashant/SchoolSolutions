namespace Application.Students.ViewModels
{
    public class PromotionExecutionResultViewModel
    {
        public int ReviewedCount { get; set; }
        public int PromotedCount { get; set; }
        public int SkippedCount { get; set; }
        public int AlreadyEnrolledCount { get; set; }
        public int NotEligibleCount { get; set; }
        public int MissingTargetCount { get; set; }
        public string TargetAcademicYearName { get; set; } = string.Empty;
    }
}
