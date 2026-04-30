namespace Application.Dashboard.ViewModels
{
    public class DashboardSummaryViewModel
    {
        public int StudentsCount { get; set; }
        public int TeachersCount { get; set; }
        public int CoursesCount { get; set; }
        public decimal FeePendingOverall { get; set; }
        public decimal FeeCollectedToday { get; set; }
        public decimal FeeCollectedOverall { get; set; }
        public int PendingFeeStudents { get; set; }
    }
}
