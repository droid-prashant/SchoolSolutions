export interface DashboardOverviewViewModel {
  userName: string;
  academicYearId: string;
  academicYearName: string;
  schoolName: string;
  schoolLogoUrl: string;
  serverDate: string;
  permissions: DashboardPermissionViewModel;
  summary: DashboardOverviewSummaryViewModel;
  attendance: DashboardAttendanceOverviewViewModel;
  fees: DashboardFeesOverviewViewModel;
  students: DashboardStudentsOverviewViewModel;
  exams: DashboardExamOverviewViewModel;
  notices: DashboardNoticeOverviewViewModel;
  activities: DashboardActivityViewModel[];
}

export interface DashboardPermissionViewModel {
  canViewStudents: boolean;
  canViewTeachers: boolean;
  canViewClasses: boolean;
  canViewCourses: boolean;
  canViewAttendance: boolean;
  canViewFees: boolean;
  canViewExams: boolean;
  canViewNotices: boolean;
}

export interface DashboardOverviewSummaryViewModel {
  totalStudents: number;
  activeStudents: number;
  totalTeachers: number;
  totalClasses: number;
  totalCourses: number;
  attendanceTakenToday: number;
  classSectionsWithoutAttendance: number;
  presentToday: number;
  absentToday: number;
  lateToday: number;
  leaveToday: number;
  halfDayToday: number;
  feesCollectedToday: number;
  feesCollectedThisMonth: number;
  pendingFees: number;
  pendingFeeStudents: number;
  noticesThisMonth: number;
  newAdmissionsThisMonth: number;
  busRequiredStudents: number;
  recentResults: number;
}

export interface DashboardAttendanceOverviewViewModel {
  present: number;
  absent: number;
  late: number;
  leave: number;
  halfDay: number;
  totalMarked: number;
  attendancePercentage: number;
  classWise: DashboardClassAttendanceViewModel[];
  recentSubmissions: DashboardRecentAttendanceViewModel[];
}

export interface DashboardClassAttendanceViewModel {
  classSectionId: string;
  className: string;
  sectionName: string;
  totalStudents: number;
  present: number;
  absent: number;
  late: number;
  leave: number;
  halfDay: number;
  totalMarked: number;
  attendanceTaken: boolean;
}

export interface DashboardRecentAttendanceViewModel {
  className: string;
  sectionName: string;
  attendanceDateEn: string;
  attendanceDateNp: string;
  recordedCount: number;
  submittedAt: string;
}

export interface DashboardFeesOverviewViewModel {
  collectedToday: number;
  collectedThisMonth: number;
  pendingAmount: number;
  pendingStudents: number;
  monthlyCollection: DashboardMonthlyAmountViewModel[];
  recentPayments: DashboardRecentPaymentViewModel[];
  classWisePending: DashboardClassAmountViewModel[];
}

export interface DashboardMonthlyAmountViewModel {
  month: string;
  amount: number;
}

export interface DashboardRecentPaymentViewModel {
  studentName: string;
  className: string;
  sectionName: string;
  amount: number;
  method: string;
  paymentDate: string;
}

export interface DashboardClassAmountViewModel {
  className: string;
  amount: number;
  students: number;
}

export interface DashboardStudentsOverviewViewModel {
  activeStudents: number;
  inactiveStudents: number;
  busRequiredStudents: number;
  genderDistribution: DashboardGenderDistributionViewModel[];
  classWiseDistribution: DashboardClassStudentDistributionViewModel[];
  sectionWiseDistribution: DashboardSectionStudentDistributionViewModel[];
  recentAdmissions: DashboardRecentAdmissionViewModel[];
}

export interface DashboardGenderDistributionViewModel {
  gender: string;
  count: number;
}

export interface DashboardClassStudentDistributionViewModel {
  className: string;
  studentCount: number;
  sectionCount: number;
  busRequiredCount: number;
}

export interface DashboardSectionStudentDistributionViewModel {
  classSectionId: string;
  className: string;
  sectionName: string;
  studentCount: number;
  busRequiredCount: number;
}

export interface DashboardRecentAdmissionViewModel {
  studentName: string;
  className: string;
  sectionName: string;
  rollNumber?: number;
  enrollmentDate: string;
}

export interface DashboardExamOverviewViewModel {
  recentResultCount: number;
  averageGpa: number;
  recentResults: DashboardRecentResultViewModel[];
}

export interface DashboardRecentResultViewModel {
  studentName: string;
  className: string;
  sectionName: string;
  examType: number;
  examTypeName: string;
  gpa: number;
  createdDate: string;
}

export interface DashboardNoticeOverviewViewModel {
  noticesThisMonth: number;
  recent: DashboardNoticeViewModel[];
}

export interface DashboardNoticeViewModel {
  title: string;
  targetAudience: string;
  noticeDate: string;
  noticeDateNp: string;
  isPublished: boolean;
}

export interface DashboardActivityViewModel {
  type: string;
  title: string;
  description: string;
  icon: string;
  severity: string;
  occurredAt: string;
}
