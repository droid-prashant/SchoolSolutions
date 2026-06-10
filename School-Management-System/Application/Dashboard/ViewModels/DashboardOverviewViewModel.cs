namespace Application.Dashboard.ViewModels
{
    public class DashboardOverviewViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public string AcademicYearId { get; set; } = string.Empty;
        public string AcademicYearName { get; set; } = string.Empty;
        public string AcademicYearStartDateNp { get; set; } = string.Empty;
        public string AcademicYearEndDateNp { get; set; } = string.Empty;
        public string AcademicYearStartDateEn { get; set; } = string.Empty;
        public string AcademicYearEndDateEn { get; set; } = string.Empty;
        public string SchoolName { get; set; } = string.Empty;
        public string SchoolLogoUrl { get; set; } = string.Empty;
        public DateTime ServerDate { get; set; }
        public DashboardPeriodViewModel Period { get; set; } = new();
        public DashboardPermissionViewModel Permissions { get; set; } = new();
        public DashboardOverviewSummaryViewModel Summary { get; set; } = new();
        public DashboardAttendanceOverviewViewModel Attendance { get; set; } = new();
        public DashboardFeesOverviewViewModel Fees { get; set; } = new();
        public DashboardStudentsOverviewViewModel Students { get; set; } = new();
        public DashboardExamOverviewViewModel Exams { get; set; } = new();
        public DashboardNoticeOverviewViewModel Notices { get; set; } = new();
        public List<DashboardActivityViewModel> Activities { get; set; } = new();
    }

    public class DashboardPeriodViewModel
    {
        public string Key { get; set; } = "this-month";
        public string Label { get; set; } = "This Month";
        public string FromDate { get; set; } = string.Empty;
        public string ToDate { get; set; } = string.Empty;
        public string FromDateNp { get; set; } = string.Empty;
        public string ToDateNp { get; set; } = string.Empty;
    }

    public class DashboardPermissionViewModel
    {
        public bool CanViewStudents { get; set; }
        public bool CanViewTeachers { get; set; }
        public bool CanViewClasses { get; set; }
        public bool CanViewCourses { get; set; }
        public bool CanViewAttendance { get; set; }
        public bool CanViewFees { get; set; }
        public bool CanViewExams { get; set; }
        public bool CanViewNotices { get; set; }
    }

    public class DashboardOverviewSummaryViewModel
    {
        public int TotalStudents { get; set; }
        public int ActiveStudents { get; set; }
        public int TotalTeachers { get; set; }
        public int TotalClasses { get; set; }
        public int TotalCourses { get; set; }
        public int AttendanceTakenToday { get; set; }
        public int ClassSectionsWithoutAttendance { get; set; }
        public int PresentToday { get; set; }
        public int AbsentToday { get; set; }
        public int LateToday { get; set; }
        public int LeaveToday { get; set; }
        public int HalfDayToday { get; set; }
        public decimal FeesCollectedToday { get; set; }
        public decimal FeesCollectedThisMonth { get; set; }
        public decimal PendingFees { get; set; }
        public int PendingFeeStudents { get; set; }
        public int NoticesThisMonth { get; set; }
        public int NewAdmissionsThisMonth { get; set; }
        public int BusRequiredStudents { get; set; }
        public int RecentResults { get; set; }
    }

    public class DashboardAttendanceOverviewViewModel
    {
        public int Present { get; set; }
        public int Absent { get; set; }
        public int Late { get; set; }
        public int Leave { get; set; }
        public int HalfDay { get; set; }
        public int TotalMarked { get; set; }
        public decimal AttendancePercentage { get; set; }
        public int PeriodPresent { get; set; }
        public int PeriodAbsent { get; set; }
        public int PeriodLate { get; set; }
        public int PeriodLeave { get; set; }
        public int PeriodHalfDay { get; set; }
        public int PeriodTotalMarked { get; set; }
        public decimal PeriodAttendancePercentage { get; set; }
        public List<DashboardClassAttendanceViewModel> ClassWise { get; set; } = new();
        public List<DashboardRecentAttendanceViewModel> RecentSubmissions { get; set; } = new();
    }

    public class DashboardClassAttendanceViewModel
    {
        public string ClassSectionId { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public int TotalStudents { get; set; }
        public int Present { get; set; }
        public int Absent { get; set; }
        public int Late { get; set; }
        public int Leave { get; set; }
        public int HalfDay { get; set; }
        public int TotalMarked { get; set; }
        public bool AttendanceTaken { get; set; }
    }

    public class DashboardRecentAttendanceViewModel
    {
        public string ClassName { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public DateOnly AttendanceDateEn { get; set; }
        public string AttendanceDateNp { get; set; } = string.Empty;
        public int RecordedCount { get; set; }
        public DateTime SubmittedAt { get; set; }
    }

    public class DashboardFeesOverviewViewModel
    {
        public decimal CollectedToday { get; set; }
        public decimal CollectedThisMonth { get; set; }
        public decimal PendingAmount { get; set; }
        public int PendingStudents { get; set; }
        public List<DashboardMonthlyAmountViewModel> MonthlyCollection { get; set; } = new();
        public List<DashboardRecentPaymentViewModel> RecentPayments { get; set; } = new();
        public List<DashboardClassAmountViewModel> ClassWisePending { get; set; } = new();
    }

    public class DashboardMonthlyAmountViewModel
    {
        public string Month { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    public class DashboardRecentPaymentViewModel
    {
        public string StudentName { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Method { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
    }

    public class DashboardClassAmountViewModel
    {
        public string ClassName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int Students { get; set; }
    }

    public class DashboardStudentsOverviewViewModel
    {
        public int ActiveStudents { get; set; }
        public int InactiveStudents { get; set; }
        public int BusRequiredStudents { get; set; }
        public List<DashboardGenderDistributionViewModel> GenderDistribution { get; set; } = new();
        public List<DashboardClassStudentDistributionViewModel> ClassWiseDistribution { get; set; } = new();
        public List<DashboardSectionStudentDistributionViewModel> SectionWiseDistribution { get; set; } = new();
        public List<DashboardRecentAdmissionViewModel> RecentAdmissions { get; set; } = new();
    }

    public class DashboardGenderDistributionViewModel
    {
        public string Gender { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class DashboardClassStudentDistributionViewModel
    {
        public string ClassName { get; set; } = string.Empty;
        public int StudentCount { get; set; }
        public int SectionCount { get; set; }
        public int BusRequiredCount { get; set; }
    }

    public class DashboardSectionStudentDistributionViewModel
    {
        public string ClassSectionId { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public int StudentCount { get; set; }
        public int BusRequiredCount { get; set; }
    }

    public class DashboardRecentAdmissionViewModel
    {
        public string StudentName { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public int? RollNumber { get; set; }
        public DateTime EnrollmentDate { get; set; }
    }

    public class DashboardExamOverviewViewModel
    {
        public int RecentResultCount { get; set; }
        public decimal AverageGpa { get; set; }
        public List<DashboardRecentResultViewModel> RecentResults { get; set; } = new();
    }

    public class DashboardRecentResultViewModel
    {
        public string StudentName { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public int ExamType { get; set; }
        public string ExamTypeName { get; set; } = string.Empty;
        public decimal Gpa { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class DashboardNoticeOverviewViewModel
    {
        public int NoticesThisMonth { get; set; }
        public List<DashboardNoticeViewModel> Recent { get; set; } = new();
    }

    public class DashboardNoticeViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string TargetAudience { get; set; } = string.Empty;
        public DateTime NoticeDate { get; set; }
        public string NoticeDateNp { get; set; } = string.Empty;
        public bool IsPublished { get; set; }
    }

    public class DashboardActivityViewModel
    {
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public DateTime OccurredAt { get; set; }
    }

    public class DashboardActivityQueryViewModel
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? Type { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class DashboardOverviewQueryViewModel
    {
        public string? PeriodKey { get; set; }
        public string? PeriodLabel { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? FromDateNp { get; set; }
        public string? ToDateNp { get; set; }
        public List<string> MonthBuckets { get; set; } = new();
    }

    public class DashboardActivityLogViewModel
    {
        public List<DashboardActivityViewModel> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
