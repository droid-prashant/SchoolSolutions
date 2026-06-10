using Application.Common.Interfaces;
using Application.Dashboard.Interfaces;
using Application.Dashboard.ViewModels;
using Domain;
using Domain.Constants;
using Domain.Enums;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Infrastructure.Services.Dashboard
{
    public class DasboardService : IDashboardService
    {
        private static readonly TimeSpan NepalOffset = new(5, 45, 0);
        private readonly IApplicationDbContext _context;
        private readonly UserResolver _userResolver;

        public DasboardService(IApplicationDbContext context, UserResolver userResolver)
        {
            _context = context;
            _userResolver = userResolver;
        }

        public async Task<int> GetCoursesCount(CancellationToken cancellationToken)
        {
            return await _context.Courses
                .AsNoTracking()
                .CountAsync(x => !x.IsDeleted, cancellationToken);
        }

        public async Task<int> GetStudentCount(CancellationToken cancellationToken)
        {
            var currentAcademicYearId = GetCurrentAcademicYearId();
            return await CurrentActiveEnrollments(currentAcademicYearId)
                .CountAsync(cancellationToken);
        }

        public async Task<int> GetTeachersCount(CancellationToken cancellationToken)
        {
            return await _context.Teachers
                .AsNoTracking()
                .CountAsync(x => !x.IsDeleted && x.IsActive, cancellationToken);
        }

        public async Task<DashboardSummaryViewModel> GetDashboardSummary(CancellationToken cancellationToken)
        {
            var currentAcademicYearId = GetCurrentAcademicYearId();
            var periods = GetDatePeriods();
            var feeTotals = await GetCurrentFeeTotals(currentAcademicYearId, cancellationToken);

            return new DashboardSummaryViewModel
            {
                StudentsCount = await GetStudentCount(cancellationToken),
                TeachersCount = await GetTeachersCount(cancellationToken),
                CoursesCount = await GetCoursesCount(cancellationToken),
                FeePendingOverall = feeTotals.Sum(x => x.PendingAmount),
                FeeCollectedOverall = feeTotals.Sum(x => x.PaidAmount),
                FeeCollectedToday = await CurrentPayments(currentAcademicYearId)
                    .Where(x => x.PaymentDate >= periods.TodayStartUtc && x.PaymentDate < periods.TomorrowStartUtc)
                    .SumAsync(x => x.AmountPaid, cancellationToken),
                PendingFeeStudents = feeTotals
                    .Where(x => x.PendingAmount > 0)
                    .Select(x => x.StudentEnrollmentId)
                    .Distinct()
                    .Count()
            };
        }

        public async Task<List<StudentsByClassViewModel>> GetStudentsByClass(CancellationToken cancellationToken)
        {
            var currentAcademicYearId = GetCurrentAcademicYearId();
            var sectionCounts = await CurrentActiveEnrollments(currentAcademicYearId)
                .GroupBy(x => new
                {
                    ClassName = x.ClassSection.ClassRoom.Name,
                    x.ClassSection.ClassRoom.OrderNumber,
                    SectionName = x.ClassSection.Section.Name
                })
                .Select(g => new
                {
                    g.Key.ClassName,
                    g.Key.OrderNumber,
                    g.Key.SectionName,
                    StudentCount = g.Count()
                })
                .OrderBy(x => x.OrderNumber)
                .ThenBy(x => x.SectionName)
                .ToListAsync(cancellationToken);

            return sectionCounts
                .GroupBy(x => new { x.ClassName, x.OrderNumber })
                .OrderBy(x => x.Key.OrderNumber)
                .Select(g => new StudentsByClassViewModel
                {
                    ClassRoom = g.Key.ClassName,
                    StudentsCountBySections = g
                        .Select(x => new StudentCountBySection
                        {
                            SectionName = x.SectionName,
                            StudentCount = x.StudentCount
                        })
                        .ToList()
                })
                .ToList();
        }

        public async Task<DashboardOverviewViewModel> GetOverview(DashboardOverviewQueryViewModel query, CancellationToken cancellationToken)
        {
            var currentAcademicYearId = GetCurrentAcademicYearId();
            var periods = GetDatePeriods(query);
            var permissions = BuildPermissions();

            var overview = new DashboardOverviewViewModel
            {
                AcademicYearId = currentAcademicYearId.ToString(),
                UserName = _userResolver.UserName,
                ServerDate = DateTime.UtcNow,
                Period = new DashboardPeriodViewModel
                {
                    Key = periods.PeriodKey,
                    Label = periods.PeriodLabel,
                    FromDate = periods.RangeStartLocal.ToString("yyyy-MM-dd"),
                    ToDate = periods.RangeEndLocalInclusive.ToString("yyyy-MM-dd"),
                    FromDateNp = periods.FromDateNp,
                    ToDateNp = periods.ToDateNp
                },
                Permissions = permissions
            };

            await FillHeaderAsync(overview, currentAcademicYearId, cancellationToken);

            if (permissions.CanViewStudents)
            {
                await FillStudentOverviewAsync(overview, currentAcademicYearId, periods, cancellationToken);
            }

            if (permissions.CanViewTeachers)
            {
                overview.Summary.TotalTeachers = await GetTeachersCount(cancellationToken);
            }

            if (permissions.CanViewClasses)
            {
                overview.Summary.TotalClasses = await _context.ClassRooms
                    .AsNoTracking()
                    .CountAsync(x => !x.IsDeleted, cancellationToken);
            }

            if (permissions.CanViewCourses)
            {
                overview.Summary.TotalCourses = await GetCoursesCount(cancellationToken);
            }

            if (permissions.CanViewAttendance)
            {
                await FillAttendanceOverviewAsync(overview, currentAcademicYearId, periods, cancellationToken);
            }

            if (permissions.CanViewFees)
            {
                await FillFeeOverviewAsync(overview, currentAcademicYearId, periods, cancellationToken);
            }

            if (permissions.CanViewExams)
            {
                await FillExamOverviewAsync(overview, currentAcademicYearId, periods, cancellationToken);
            }

            if (permissions.CanViewNotices)
            {
                await FillNoticeOverviewAsync(overview, periods, cancellationToken);
            }

            overview.Activities = BuildActivities(overview);
            return overview;
        }

        public async Task<DashboardActivityLogViewModel> GetActivities(DashboardActivityQueryViewModel query, CancellationToken cancellationToken)
        {
            var currentAcademicYearId = GetCurrentAcademicYearId();
            var permissions = BuildPermissions();
            var page = Math.Max(query.Page, 1);
            var pageSize = Math.Clamp(query.PageSize <= 0 ? 20 : query.PageSize, 1, 100);
            var skip = (page - 1) * pageSize;
            var fetchCount = skip + pageSize;
            var fromUtc = query.FromDate.HasValue ? NepalLocalDateToUtc(query.FromDate.Value.Date) : (DateTime?)null;
            var toUtc = query.ToDate.HasValue ? NepalLocalDateToUtc(query.ToDate.Value.Date.AddDays(1)) : (DateTime?)null;
            var totalCount = 0;
            var activities = new List<DashboardActivityViewModel>();

            if (permissions.CanViewStudents && MatchesActivityType(query.Type, "Student"))
            {
                var admissionsQuery = CurrentActiveEnrollments(currentAcademicYearId);
                if (fromUtc.HasValue)
                {
                    admissionsQuery = admissionsQuery.Where(x => x.EnrollmentDate >= fromUtc.Value);
                }

                if (toUtc.HasValue)
                {
                    admissionsQuery = admissionsQuery.Where(x => x.EnrollmentDate < toUtc.Value);
                }

                totalCount += await admissionsQuery.CountAsync(cancellationToken);

                var admissions = await admissionsQuery
                    .OrderByDescending(x => x.EnrollmentDate)
                    .Take(fetchCount)
                    .Select(x => new
                    {
                        StudentName = (x.Student.FirstName + " " + x.Student.LastName).Trim(),
                        ClassName = x.ClassSection.ClassRoom.Name,
                        SectionName = x.ClassSection.Section.Name,
                        x.EnrollmentDate
                    })
                    .ToListAsync(cancellationToken);

                activities.AddRange(admissions.Select(x => new DashboardActivityViewModel
                {
                    Type = "Student",
                    Title = "New student admitted",
                    Description = $"{x.StudentName} in {x.ClassName} - {x.SectionName}",
                    Icon = "pi pi-user-plus",
                    Severity = "success",
                    OccurredAt = x.EnrollmentDate
                }));
            }

            if (permissions.CanViewAttendance && MatchesActivityType(query.Type, "Attendance"))
            {
                var attendanceQuery = _context.StudentAttendances
                    .AsNoTracking()
                    .Where(x => x.AcademicYearId == currentAcademicYearId && !x.IsDeleted)
                    .GroupBy(x => new
                    {
                        x.ClassSectionId,
                        ClassName = x.ClassSection.ClassRoom.Name,
                        SectionName = x.ClassSection.Section.Name,
                        x.AttendanceDateEn,
                        x.AttendanceDateNp
                    })
                    .Select(g => new
                    {
                        g.Key.ClassName,
                        g.Key.SectionName,
                        g.Key.AttendanceDateEn,
                        g.Key.AttendanceDateNp,
                        RecordedCount = g.Count(),
                        SubmittedAt = g.Max(x => x.CreatedDate)
                    });

                if (fromUtc.HasValue)
                {
                    attendanceQuery = attendanceQuery.Where(x => x.SubmittedAt >= fromUtc.Value);
                }

                if (toUtc.HasValue)
                {
                    attendanceQuery = attendanceQuery.Where(x => x.SubmittedAt < toUtc.Value);
                }

                totalCount += await attendanceQuery.CountAsync(cancellationToken);

                var attendanceRows = await attendanceQuery
                    .OrderByDescending(x => x.SubmittedAt)
                    .Take(fetchCount)
                    .ToListAsync(cancellationToken);

                activities.AddRange(attendanceRows.Select(x => new DashboardActivityViewModel
                {
                    Type = "Attendance",
                    Title = "Attendance submitted",
                    Description = $"{x.ClassName} - {x.SectionName}: {x.RecordedCount} attendance records",
                    Icon = "pi pi-check-square",
                    Severity = "primary",
                    OccurredAt = x.SubmittedAt
                }));
            }

            if (permissions.CanViewFees && MatchesActivityType(query.Type, "Fee"))
            {
                var paymentsQuery = CurrentPayments(currentAcademicYearId);
                if (fromUtc.HasValue)
                {
                    paymentsQuery = paymentsQuery.Where(x => x.PaymentDate >= fromUtc.Value);
                }

                if (toUtc.HasValue)
                {
                    paymentsQuery = paymentsQuery.Where(x => x.PaymentDate < toUtc.Value);
                }

                totalCount += await paymentsQuery.CountAsync(cancellationToken);

                var payments = await paymentsQuery
                    .OrderByDescending(x => x.PaymentDate)
                    .Take(fetchCount)
                    .Select(x => new
                    {
                        StudentName = (x.StudentFee.StudentEnrollment.Student.FirstName + " " + x.StudentFee.StudentEnrollment.Student.LastName).Trim(),
                        ClassName = x.StudentFee.StudentEnrollment.ClassSection.ClassRoom.Name,
                        SectionName = x.StudentFee.StudentEnrollment.ClassSection.Section.Name,
                        Amount = x.AmountPaid,
                        x.Method,
                        x.PaymentDate
                    })
                    .ToListAsync(cancellationToken);

                activities.AddRange(payments.Select(x => new DashboardActivityViewModel
                {
                    Type = "Fee",
                    Title = "Fee payment received",
                    Description = $"Rs {x.Amount:N0} from {x.StudentName} ({x.ClassName} - {x.SectionName}) through {x.Method}",
                    Icon = "pi pi-wallet",
                    Severity = "info",
                    OccurredAt = x.PaymentDate
                }));
            }

            if (permissions.CanViewExams && MatchesActivityType(query.Type, "Exam"))
            {
                var resultsQuery = _context.ExamResults
                    .AsNoTracking()
                    .Where(x => x.StudentEnrollment.AcademicYearId == currentAcademicYearId &&
                                !x.IsDeleted &&
                                !x.StudentEnrollment.IsDeleted &&
                                !x.StudentEnrollment.Student.IsDeleted);

                if (fromUtc.HasValue)
                {
                    resultsQuery = resultsQuery.Where(x => x.CreatedDate >= fromUtc.Value);
                }

                if (toUtc.HasValue)
                {
                    resultsQuery = resultsQuery.Where(x => x.CreatedDate < toUtc.Value);
                }

                totalCount += await resultsQuery.CountAsync(cancellationToken);

                var results = await resultsQuery
                    .OrderByDescending(x => x.CreatedDate)
                    .Take(fetchCount)
                    .Select(x => new
                    {
                        StudentName = (x.StudentEnrollment.Student.FirstName + " " + x.StudentEnrollment.Student.LastName).Trim(),
                        ClassName = x.StudentEnrollment.ClassSection.ClassRoom.Name,
                        SectionName = x.StudentEnrollment.ClassSection.Section.Name,
                        x.ExamType,
                        x.GPA,
                        x.CreatedDate
                    })
                    .ToListAsync(cancellationToken);

                activities.AddRange(results.Select(x => new DashboardActivityViewModel
                {
                    Type = "Exam",
                    Title = "Marks entered",
                    Description = $"{GetExamTypeName(x.ExamType)} result for {x.StudentName} ({x.ClassName} - {x.SectionName}), GPA {x.GPA:N2}",
                    Icon = "pi pi-chart-line",
                    Severity = "warning",
                    OccurredAt = x.CreatedDate
                }));
            }

            if (permissions.CanViewNotices && MatchesActivityType(query.Type, "Notice"))
            {
                var noticesQuery = _context.Notices
                    .AsNoTracking()
                    .Where(x => !x.IsDeleted);

                if (fromUtc.HasValue)
                {
                    noticesQuery = noticesQuery.Where(x => x.NoticeDate >= fromUtc.Value);
                }

                if (toUtc.HasValue)
                {
                    noticesQuery = noticesQuery.Where(x => x.NoticeDate < toUtc.Value);
                }

                totalCount += await noticesQuery.CountAsync(cancellationToken);

                var notices = await noticesQuery
                    .OrderByDescending(x => x.NoticeDate)
                    .Take(fetchCount)
                    .Select(x => new
                    {
                        x.Title,
                        x.IsPublished,
                        x.NoticeDate
                    })
                    .ToListAsync(cancellationToken);

                activities.AddRange(notices.Select(x => new DashboardActivityViewModel
                {
                    Type = "Notice",
                    Title = x.IsPublished ? "Notice published" : "Notice drafted",
                    Description = x.Title,
                    Icon = "pi pi-send",
                    Severity = x.IsPublished ? "success" : "warning",
                    OccurredAt = x.NoticeDate
                }));
            }

            return new DashboardActivityLogViewModel
            {
                Items = activities
                    .Where(x => x.OccurredAt != default)
                    .OrderByDescending(x => x.OccurredAt)
                    .Skip(skip)
                    .Take(pageSize)
                    .ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        private async Task FillHeaderAsync(DashboardOverviewViewModel overview, Guid academicYearId, CancellationToken cancellationToken)
        {
            var academicYear = await _context.AcademicYears
                .AsNoTracking()
                .Where(x => x.Id == academicYearId)
                .Select(x => new { x.YearName, x.StartDateNp, x.EndDateNp, x.StartDateEn, x.EndDateEn })
                .FirstOrDefaultAsync(cancellationToken);

            var school = await _context.Schools
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.IsActive)
                .ThenByDescending(x => x.CreatedDate)
                .Select(x => new { x.Name, x.LogoUrl })
                .FirstOrDefaultAsync(cancellationToken);

            overview.AcademicYearName = academicYear?.YearName ?? string.Empty;
            overview.AcademicYearStartDateNp = academicYear?.StartDateNp ?? string.Empty;
            overview.AcademicYearEndDateNp = academicYear?.EndDateNp ?? string.Empty;
            overview.AcademicYearStartDateEn = academicYear?.StartDateEn ?? string.Empty;
            overview.AcademicYearEndDateEn = academicYear?.EndDateEn ?? string.Empty;
            overview.SchoolName = school?.Name ?? string.Empty;
            overview.SchoolLogoUrl = school?.LogoUrl ?? string.Empty;
        }

        private async Task FillStudentOverviewAsync(
            DashboardOverviewViewModel overview,
            Guid academicYearId,
            DashboardDatePeriods periods,
            CancellationToken cancellationToken)
        {
            var activeEnrollmentQuery = CurrentActiveEnrollments(academicYearId);

            overview.Summary.TotalStudents = await _context.Students
                .AsNoTracking()
                .CountAsync(x => !x.IsDeleted, cancellationToken);

            overview.Summary.ActiveStudents = await activeEnrollmentQuery.CountAsync(cancellationToken);
            overview.Summary.NewAdmissionsThisMonth = await activeEnrollmentQuery
                .CountAsync(x => x.EnrollmentDate >= periods.RangeStartUtc && x.EnrollmentDate < periods.RangeEndUtc, cancellationToken);
            overview.Summary.BusRequiredStudents = await activeEnrollmentQuery
                .CountAsync(x => x.IsBusRequired, cancellationToken);

            overview.Students.ActiveStudents = overview.Summary.ActiveStudents;
            overview.Students.InactiveStudents = await _context.StudentEnrollments
                .AsNoTracking()
                .CountAsync(x => x.AcademicYearId == academicYearId &&
                                 !x.IsDeleted &&
                                 !x.Student.IsDeleted &&
                                 !x.IsActive, cancellationToken);
            overview.Students.BusRequiredStudents = overview.Summary.BusRequiredStudents;

            var genderDistribution = await activeEnrollmentQuery
                .GroupBy(x => x.Student.Gender)
                .Select(g => new
                {
                    Gender = g.Key,
                    Count = g.Count()
                })
                .OrderBy(x => x.Gender)
                .ToListAsync(cancellationToken);

            overview.Students.GenderDistribution = genderDistribution
                .Select(x => new DashboardGenderDistributionViewModel
                {
                    Gender = GetGenderName(x.Gender),
                    Count = x.Count
                })
                .ToList();

            var classDistribution = await activeEnrollmentQuery
                .GroupBy(x => new
                {
                    ClassName = x.ClassSection.ClassRoom.Name,
                    x.ClassSection.ClassRoom.OrderNumber
                })
                .Select(g => new
                {
                    g.Key.ClassName,
                    g.Key.OrderNumber,
                    StudentCount = g.Count(),
                    SectionCount = g.Select(x => x.ClassSectionId).Distinct().Count(),
                    BusRequiredCount = g.Count(x => x.IsBusRequired)
                })
                .OrderBy(x => x.OrderNumber)
                .ToListAsync(cancellationToken);

            overview.Students.ClassWiseDistribution = classDistribution
                .Select(x => new DashboardClassStudentDistributionViewModel
                {
                    ClassName = x.ClassName,
                    StudentCount = x.StudentCount,
                    SectionCount = x.SectionCount,
                    BusRequiredCount = x.BusRequiredCount
                })
                .ToList();

            var sectionDistribution = await activeEnrollmentQuery
                .GroupBy(x => new
                {
                    x.ClassSectionId,
                    ClassName = x.ClassSection.ClassRoom.Name,
                    SectionName = x.ClassSection.Section.Name,
                    x.ClassSection.ClassRoom.OrderNumber
                })
                .Select(g => new
                {
                    g.Key.ClassSectionId,
                    g.Key.ClassName,
                    g.Key.SectionName,
                    g.Key.OrderNumber,
                    StudentCount = g.Count(),
                    BusRequiredCount = g.Count(x => x.IsBusRequired)
                })
                .OrderBy(x => x.OrderNumber)
                .ThenBy(x => x.SectionName)
                .ToListAsync(cancellationToken);

            overview.Students.SectionWiseDistribution = sectionDistribution
                .Select(x => new DashboardSectionStudentDistributionViewModel
                {
                    ClassSectionId = x.ClassSectionId.ToString(),
                    ClassName = x.ClassName,
                    SectionName = x.SectionName,
                    StudentCount = x.StudentCount,
                    BusRequiredCount = x.BusRequiredCount
                })
                .ToList();

            overview.Students.RecentAdmissions = await activeEnrollmentQuery
                .Where(x => x.EnrollmentDate >= periods.RangeStartUtc && x.EnrollmentDate < periods.RangeEndUtc)
                .OrderByDescending(x => x.EnrollmentDate)
                .ThenByDescending(x => x.CreatedDate)
                .Take(6)
                .Select(x => new DashboardRecentAdmissionViewModel
                {
                    StudentName = (x.Student.FirstName + " " + x.Student.LastName).Trim(),
                    ClassName = x.ClassSection.ClassRoom.Name,
                    SectionName = x.ClassSection.Section.Name,
                    RollNumber = x.RollNumber,
                    EnrollmentDate = x.EnrollmentDate
                })
                .ToListAsync(cancellationToken);
        }

        private async Task FillAttendanceOverviewAsync(
            DashboardOverviewViewModel overview,
            Guid academicYearId,
            DashboardDatePeriods periods,
            CancellationToken cancellationToken)
        {
            var activeClassSections = await CurrentActiveEnrollments(academicYearId)
                .GroupBy(x => new
                {
                    x.ClassSectionId,
                    ClassName = x.ClassSection.ClassRoom.Name,
                    SectionName = x.ClassSection.Section.Name,
                    x.ClassSection.ClassRoom.OrderNumber
                })
                .Select(g => new ActiveClassSectionRow
                {
                    ClassSectionId = g.Key.ClassSectionId,
                    ClassName = g.Key.ClassName,
                    SectionName = g.Key.SectionName,
                    OrderNumber = g.Key.OrderNumber,
                    TotalStudents = g.Count()
                })
                .OrderBy(x => x.OrderNumber)
                .ThenBy(x => x.SectionName)
                .ToListAsync(cancellationToken);

            var todayRows = await _context.StudentAttendances
                .AsNoTracking()
                .Where(x => x.AcademicYearId == academicYearId &&
                            x.AttendanceDateEn == periods.TodayEn &&
                            !x.IsDeleted)
                .Select(x => new AttendanceStatusRow
                {
                    ClassSectionId = x.ClassSectionId,
                    Status = x.Status
                })
                .ToListAsync(cancellationToken);

            overview.Attendance.Present = todayRows.Count(x => x.Status == StudentAttendanceStatus.Present);
            overview.Attendance.Absent = todayRows.Count(x => x.Status == StudentAttendanceStatus.Absent);
            overview.Attendance.Late = todayRows.Count(x => x.Status == StudentAttendanceStatus.Late);
            overview.Attendance.Leave = todayRows.Count(x => x.Status == StudentAttendanceStatus.Leave);
            overview.Attendance.HalfDay = todayRows.Count(x => x.Status == StudentAttendanceStatus.HalfDay);
            overview.Attendance.TotalMarked = todayRows.Count;
            overview.Attendance.AttendancePercentage = CalculateAttendancePercentage(todayRows.Select(x => x.Status).ToList());

            var periodStartEn = DateOnly.FromDateTime(periods.RangeStartLocal);
            var periodEndEn = DateOnly.FromDateTime(periods.RangeEndLocalInclusive);
            var periodRows = await _context.StudentAttendances
                .AsNoTracking()
                .Where(x => x.AcademicYearId == academicYearId &&
                            x.AttendanceDateEn >= periodStartEn &&
                            x.AttendanceDateEn <= periodEndEn &&
                            !x.IsDeleted)
                .Select(x => x.Status)
                .ToListAsync(cancellationToken);

            overview.Attendance.PeriodPresent = periodRows.Count(x => x == StudentAttendanceStatus.Present);
            overview.Attendance.PeriodAbsent = periodRows.Count(x => x == StudentAttendanceStatus.Absent);
            overview.Attendance.PeriodLate = periodRows.Count(x => x == StudentAttendanceStatus.Late);
            overview.Attendance.PeriodLeave = periodRows.Count(x => x == StudentAttendanceStatus.Leave);
            overview.Attendance.PeriodHalfDay = periodRows.Count(x => x == StudentAttendanceStatus.HalfDay);
            overview.Attendance.PeriodTotalMarked = periodRows.Count;
            overview.Attendance.PeriodAttendancePercentage = CalculateAttendancePercentage(periodRows);

            var attendedClassSectionIds = todayRows.Select(x => x.ClassSectionId).Distinct().ToHashSet();
            overview.Summary.AttendanceTakenToday = attendedClassSectionIds.Count;
            overview.Summary.ClassSectionsWithoutAttendance = Math.Max(activeClassSections.Count - attendedClassSectionIds.Count, 0);
            overview.Summary.PresentToday = overview.Attendance.Present;
            overview.Summary.AbsentToday = overview.Attendance.Absent;
            overview.Summary.LateToday = overview.Attendance.Late;
            overview.Summary.LeaveToday = overview.Attendance.Leave;
            overview.Summary.HalfDayToday = overview.Attendance.HalfDay;

            var attendanceBySection = todayRows
                .GroupBy(x => x.ClassSectionId)
                .ToDictionary(x => x.Key, x => x.ToList());

            overview.Attendance.ClassWise = activeClassSections
                .Select(section =>
                {
                    attendanceBySection.TryGetValue(section.ClassSectionId, out var rows);
                    rows ??= [];

                    return new DashboardClassAttendanceViewModel
                    {
                        ClassSectionId = section.ClassSectionId.ToString(),
                        ClassName = section.ClassName,
                        SectionName = section.SectionName,
                        TotalStudents = section.TotalStudents,
                        Present = rows.Count(x => x.Status == StudentAttendanceStatus.Present),
                        Absent = rows.Count(x => x.Status == StudentAttendanceStatus.Absent),
                        Late = rows.Count(x => x.Status == StudentAttendanceStatus.Late),
                        Leave = rows.Count(x => x.Status == StudentAttendanceStatus.Leave),
                        HalfDay = rows.Count(x => x.Status == StudentAttendanceStatus.HalfDay),
                        TotalMarked = rows.Count,
                        AttendanceTaken = rows.Count > 0
                    };
                })
                .ToList();

            overview.Attendance.RecentSubmissions = await _context.StudentAttendances
                .AsNoTracking()
                .Where(x => x.AcademicYearId == academicYearId &&
                            !x.IsDeleted &&
                            x.AttendanceDateEn >= periodStartEn &&
                            x.AttendanceDateEn <= periodEndEn)
                .GroupBy(x => new
                {
                    x.ClassSectionId,
                    ClassName = x.ClassSection.ClassRoom.Name,
                    SectionName = x.ClassSection.Section.Name,
                    x.AttendanceDateEn,
                    x.AttendanceDateNp
                })
                .Select(g => new DashboardRecentAttendanceViewModel
                {
                    ClassName = g.Key.ClassName,
                    SectionName = g.Key.SectionName,
                    AttendanceDateEn = g.Key.AttendanceDateEn,
                    AttendanceDateNp = g.Key.AttendanceDateNp,
                    RecordedCount = g.Count(),
                    SubmittedAt = g.Max(x => x.CreatedDate)
                })
                .OrderByDescending(x => x.SubmittedAt)
                .Take(6)
                .ToListAsync(cancellationToken);
        }

        private async Task FillFeeOverviewAsync(
            DashboardOverviewViewModel overview,
            Guid academicYearId,
            DashboardDatePeriods periods,
            CancellationToken cancellationToken)
        {
            var feeTotals = await GetCurrentFeeTotals(academicYearId, cancellationToken);

            overview.Fees.PendingAmount = feeTotals.Sum(x => x.PendingAmount);
            overview.Fees.PendingStudents = feeTotals
                .Where(x => x.PendingAmount > 0)
                .Select(x => x.StudentEnrollmentId)
                .Distinct()
                .Count();

            overview.Fees.CollectedToday = await CurrentPayments(academicYearId)
                .Where(x => x.PaymentDate >= periods.TodayStartUtc && x.PaymentDate < periods.TomorrowStartUtc)
                .SumAsync(x => x.AmountPaid, cancellationToken);

            overview.Fees.CollectedThisMonth = await CurrentPayments(academicYearId)
                .Where(x => x.PaymentDate >= periods.RangeStartUtc && x.PaymentDate < periods.RangeEndUtc)
                .SumAsync(x => x.AmountPaid, cancellationToken);

            overview.Summary.FeesCollectedToday = overview.Fees.CollectedToday;
            overview.Summary.FeesCollectedThisMonth = overview.Fees.CollectedThisMonth;
            overview.Summary.PendingFees = overview.Fees.PendingAmount;
            overview.Summary.PendingFeeStudents = overview.Fees.PendingStudents;

            var monthlyPayments = await CurrentPayments(academicYearId)
                .Where(x => x.PaymentDate >= periods.MonthBuckets.First().StartUtc &&
                            x.PaymentDate < periods.MonthBuckets.Last().EndUtc)
                .Select(x => new { x.PaymentDate, x.AmountPaid })
                .ToListAsync(cancellationToken);

            overview.Fees.MonthlyCollection = periods.MonthBuckets
                .Select(bucket => new DashboardMonthlyAmountViewModel
                {
                    Month = bucket.Label,
                    Amount = monthlyPayments
                        .Where(payment => payment.PaymentDate >= bucket.StartUtc && payment.PaymentDate < bucket.EndUtc)
                        .Sum(payment => payment.AmountPaid)
                })
                .ToList();

            overview.Fees.RecentPayments = await CurrentPayments(academicYearId)
                .Where(x => x.PaymentDate >= periods.RangeStartUtc && x.PaymentDate < periods.RangeEndUtc)
                .OrderByDescending(x => x.PaymentDate)
                .Take(6)
                .Select(x => new DashboardRecentPaymentViewModel
                {
                    StudentName = (x.StudentFee.StudentEnrollment.Student.FirstName + " " + x.StudentFee.StudentEnrollment.Student.LastName).Trim(),
                    ClassName = x.StudentFee.StudentEnrollment.ClassSection.ClassRoom.Name,
                    SectionName = x.StudentFee.StudentEnrollment.ClassSection.Section.Name,
                    Amount = x.AmountPaid,
                    Method = x.Method,
                    PaymentDate = x.PaymentDate
                })
                .ToListAsync(cancellationToken);

            overview.Fees.ClassWisePending = feeTotals
                .Where(x => x.PendingAmount > 0)
                .GroupBy(x => new { x.ClassName, x.ClassOrder })
                .OrderBy(x => x.Key.ClassOrder)
                .Select(g => new DashboardClassAmountViewModel
                {
                    ClassName = g.Key.ClassName,
                    Amount = g.Sum(x => x.PendingAmount),
                    Students = g.Select(x => x.StudentEnrollmentId).Distinct().Count()
                })
                .Take(6)
                .ToList();
        }

        private async Task FillExamOverviewAsync(
            DashboardOverviewViewModel overview,
            Guid academicYearId,
            DashboardDatePeriods periods,
            CancellationToken cancellationToken)
        {
            var resultQuery = _context.ExamResults
                .AsNoTracking()
                .Where(x => x.StudentEnrollment.AcademicYearId == academicYearId &&
                            !x.IsDeleted &&
                            !x.StudentEnrollment.IsDeleted &&
                            !x.StudentEnrollment.Student.IsDeleted &&
                            x.CreatedDate >= periods.RangeStartUtc &&
                            x.CreatedDate < periods.RangeEndUtc);

            overview.Exams.RecentResultCount = await resultQuery.CountAsync(cancellationToken);
            overview.Exams.AverageGpa = Math.Round(
                await resultQuery.Select(x => (decimal?)x.GPA).AverageAsync(cancellationToken) ?? 0m,
                2);
            overview.Summary.RecentResults = overview.Exams.RecentResultCount;

            var recentResults = await resultQuery
                .OrderByDescending(x => x.CreatedDate)
                .Take(6)
                .Select(x => new
                {
                    StudentName = (x.StudentEnrollment.Student.FirstName + " " + x.StudentEnrollment.Student.LastName).Trim(),
                    ClassName = x.StudentEnrollment.ClassSection.ClassRoom.Name,
                    SectionName = x.StudentEnrollment.ClassSection.Section.Name,
                    x.ExamType,
                    x.GPA,
                    x.CreatedDate
                })
                .ToListAsync(cancellationToken);

            overview.Exams.RecentResults = recentResults
                .Select(x => new DashboardRecentResultViewModel
                {
                    StudentName = x.StudentName,
                    ClassName = x.ClassName,
                    SectionName = x.SectionName,
                    ExamType = x.ExamType,
                    ExamTypeName = GetExamTypeName(x.ExamType),
                    Gpa = x.GPA,
                    CreatedDate = x.CreatedDate
                })
                .ToList();
        }

        private async Task FillNoticeOverviewAsync(
            DashboardOverviewViewModel overview,
            DashboardDatePeriods periods,
            CancellationToken cancellationToken)
        {
            overview.Notices.NoticesThisMonth = await _context.Notices
                .AsNoTracking()
                .CountAsync(x => !x.IsDeleted &&
                                 x.NoticeDate >= periods.RangeStartUtc &&
                                 x.NoticeDate < periods.RangeEndUtc, cancellationToken);

            overview.Summary.NoticesThisMonth = overview.Notices.NoticesThisMonth;

            overview.Notices.Recent = await _context.Notices
                .AsNoTracking()
                .Where(x => !x.IsDeleted &&
                            x.NoticeDate >= periods.RangeStartUtc &&
                            x.NoticeDate < periods.RangeEndUtc)
                .OrderByDescending(x => x.NoticeDate)
                .Take(6)
                .Select(x => new DashboardNoticeViewModel
                {
                    Title = x.Title,
                    TargetAudience = x.TargetAudience,
                    NoticeDate = x.NoticeDate,
                    NoticeDateNp = x.NoticeDateNp,
                    IsPublished = x.IsPublished
                })
                .ToListAsync(cancellationToken);
        }

        private List<DashboardActivityViewModel> BuildActivities(DashboardOverviewViewModel overview)
        {
            var activities = new List<DashboardActivityViewModel>();

            if (overview.Permissions.CanViewStudents)
            {
                activities.AddRange(overview.Students.RecentAdmissions.Select(x => new DashboardActivityViewModel
                {
                    Type = "Admission",
                    Title = x.StudentName,
                    Description = $"Enrolled in {x.ClassName} - {x.SectionName}",
                    Icon = "pi pi-user-plus",
                    Severity = "success",
                    OccurredAt = x.EnrollmentDate
                }));
            }

            if (overview.Permissions.CanViewFees)
            {
                activities.AddRange(overview.Fees.RecentPayments.Select(x => new DashboardActivityViewModel
                {
                    Type = "Fee",
                    Title = x.StudentName,
                    Description = $"Rs {x.Amount:N0} collected through {x.Method}",
                    Icon = "pi pi-wallet",
                    Severity = "info",
                    OccurredAt = x.PaymentDate
                }));
            }

            if (overview.Permissions.CanViewAttendance)
            {
                activities.AddRange(overview.Attendance.RecentSubmissions.Select(x => new DashboardActivityViewModel
                {
                    Type = "Attendance",
                    Title = $"{x.ClassName} - {x.SectionName}",
                    Description = $"{x.RecordedCount} attendance records submitted",
                    Icon = "pi pi-check-square",
                    Severity = "primary",
                    OccurredAt = x.SubmittedAt
                }));
            }

            if (overview.Permissions.CanViewExams)
            {
                activities.AddRange(overview.Exams.RecentResults.Select(x => new DashboardActivityViewModel
                {
                    Type = "Result",
                    Title = x.StudentName,
                    Description = $"{x.ExamTypeName} result recorded with GPA {x.Gpa:N2}",
                    Icon = "pi pi-chart-line",
                    Severity = "warning",
                    OccurredAt = x.CreatedDate
                }));
            }

            if (overview.Permissions.CanViewNotices)
            {
                activities.AddRange(overview.Notices.Recent.Select(x => new DashboardActivityViewModel
                {
                    Type = "Notice",
                    Title = x.Title,
                    Description = x.IsPublished ? "Published notice" : "Draft notice",
                    Icon = "pi pi-send",
                    Severity = x.IsPublished ? "success" : "warning",
                    OccurredAt = x.NoticeDate
                }));
            }

            return activities
                .Where(x => x.OccurredAt != default)
                .OrderByDescending(x => x.OccurredAt)
                .Take(8)
                .ToList();
        }

        private IQueryable<StudentEnrollment> CurrentActiveEnrollments(Guid academicYearId)
        {
            return _context.StudentEnrollments
                .AsNoTracking()
                .Where(x => x.AcademicYearId == academicYearId &&
                            x.IsActive &&
                            !x.IsDeleted &&
                            !x.Student.IsDeleted);
        }

        private IQueryable<Payment> CurrentPayments(Guid academicYearId)
        {
            return _context.Payments
                .AsNoTracking()
                .Where(x => !x.IsDeleted &&
                            !x.StudentFee.IsDeleted &&
                            x.StudentFee.StudentEnrollment.AcademicYearId == academicYearId &&
                            !x.StudentFee.StudentEnrollment.IsDeleted &&
                            !x.StudentFee.StudentEnrollment.Student.IsDeleted);
        }

        private async Task<List<FeeTotalRow>> GetCurrentFeeTotals(Guid academicYearId, CancellationToken cancellationToken)
        {
            return await _context.StudentFees
                .AsNoTracking()
                .Where(x => !x.IsDeleted &&
                            x.StudentEnrollment.AcademicYearId == academicYearId &&
                            !x.StudentEnrollment.IsDeleted &&
                            !x.StudentEnrollment.Student.IsDeleted)
                .Select(x => new FeeTotalRow
                {
                    StudentEnrollmentId = x.StudentEnrollmentId,
                    ClassName = x.StudentEnrollment.ClassSection.ClassRoom.Name,
                    ClassOrder = x.StudentEnrollment.ClassSection.ClassRoom.OrderNumber,
                    NetAmount = x.Amount -
                                x.FeeAdjustments.Where(a => !a.IsDeleted).Sum(a => a.DiscountAmount) +
                                x.FeeAdjustments.Where(a => !a.IsDeleted).Sum(a => a.FineAmount) +
                                x.FeeAdjustments.Where(a => !a.IsDeleted).Sum(a => a.EducationTaxAmount),
                    PaidAmount = x.Payments.Where(p => !p.IsDeleted).Sum(p => p.AmountPaid)
                })
                .ToListAsync(cancellationToken);
        }

        private DashboardPermissionViewModel BuildPermissions()
        {
            return new DashboardPermissionViewModel
            {
                CanViewStudents = _userResolver.HasAnyPermission(
                    PermissionNames.StudentView,
                    PermissionNames.StudentCreate,
                    PermissionNames.StudentUpdate,
                    PermissionNames.StudentDelete),
                CanViewTeachers = _userResolver.HasAnyPermission(
                    PermissionNames.TeacherView,
                    PermissionNames.TeacherManage),
                CanViewClasses = _userResolver.HasAnyPermission(
                    PermissionNames.ClassManage,
                    PermissionNames.StudentView),
                CanViewCourses = _userResolver.HasAnyPermission(
                    PermissionNames.CourseManage,
                    PermissionNames.ExamMarksEntry,
                    PermissionNames.SubjectMarksEntry),
                CanViewAttendance = _userResolver.HasAnyPermission(
                    PermissionNames.StudentAttendanceView,
                    PermissionNames.StudentAttendanceTake,
                    PermissionNames.StudentAttendanceEdit,
                    PermissionNames.StudentAttendanceReport),
                CanViewFees = _userResolver.HasAnyPermission(
                    PermissionNames.FeeView,
                    PermissionNames.FeeCreate),
                CanViewExams = _userResolver.HasAnyPermission(
                    PermissionNames.ResultView,
                    PermissionNames.ExamMarksEntry,
                    PermissionNames.SubjectMarksEntry),
                CanViewNotices = _userResolver.HasAnyPermission(PermissionNames.NoticeManage)
            };
        }

        private Guid GetCurrentAcademicYearId()
        {
            return _userResolver.GetAcademicYearGuidOrThrow();
        }

        private static DashboardDatePeriods GetDatePeriods(DashboardOverviewQueryViewModel? query = null)
        {
            var nepalNow = DateTime.UtcNow.Add(NepalOffset);
            var todayStartNepal = new DateTime(nepalNow.Year, nepalNow.Month, nepalNow.Day);
            var monthStartNepal = new DateTime(nepalNow.Year, nepalNow.Month, 1);
            var rangeStartLocal = query?.FromDate?.Date ?? monthStartNepal;
            var rangeEndLocalInclusive = query?.ToDate?.Date ?? monthStartNepal.AddMonths(1).AddDays(-1);

            if (rangeEndLocalInclusive < rangeStartLocal)
            {
                rangeStartLocal = monthStartNepal;
                rangeEndLocalInclusive = monthStartNepal.AddMonths(1).AddDays(-1);
            }

            var monthBuckets = ParseMonthBuckets(query?.MonthBuckets);

            return new DashboardDatePeriods
            {
                PeriodKey = string.IsNullOrWhiteSpace(query?.PeriodKey) ? "this-month" : query.PeriodKey,
                PeriodLabel = string.IsNullOrWhiteSpace(query?.PeriodLabel) ? "This Month" : query.PeriodLabel,
                FromDateNp = query?.FromDateNp ?? string.Empty,
                ToDateNp = query?.ToDateNp ?? string.Empty,
                TodayEn = DateOnly.FromDateTime(nepalNow),
                TodayStartUtc = NepalLocalDateToUtc(todayStartNepal),
                TomorrowStartUtc = NepalLocalDateToUtc(todayStartNepal.AddDays(1)),
                RangeStartLocal = rangeStartLocal,
                RangeEndLocalInclusive = rangeEndLocalInclusive,
                RangeStartUtc = NepalLocalDateToUtc(rangeStartLocal),
                RangeEndUtc = NepalLocalDateToUtc(rangeEndLocalInclusive.AddDays(1)),
                MonthBuckets = monthBuckets.Count > 0
                    ? monthBuckets
                    : BuildFallbackMonthBuckets(monthStartNepal.AddMonths(-5), 6)
            };
        }

        private static DateTime NepalLocalDateToUtc(DateTime nepalLocalDate)
        {
            return DateTime.SpecifyKind(nepalLocalDate.Date.Subtract(NepalOffset), DateTimeKind.Utc);
        }

        private static List<DashboardMonthBucket> ParseMonthBuckets(List<string>? rawBuckets)
        {
            if (rawBuckets is null || rawBuckets.Count == 0)
            {
                return [];
            }

            return rawBuckets
                .Select(ParseMonthBucket)
                .Where(x => x is not null)
                .Select(x => x!)
                .OrderBy(x => x.StartUtc)
                .ToList();
        }

        private static DashboardMonthBucket? ParseMonthBucket(string rawBucket)
        {
            var parts = rawBucket.Split('|', StringSplitOptions.TrimEntries);
            if (parts.Length != 3 ||
                string.IsNullOrWhiteSpace(parts[0]) ||
                !DateTime.TryParseExact(parts[1], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var startDate) ||
                !DateTime.TryParseExact(parts[2], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var endDate) ||
                endDate < startDate)
            {
                return null;
            }

            return new DashboardMonthBucket
            {
                Label = parts[0],
                StartUtc = NepalLocalDateToUtc(startDate),
                EndUtc = NepalLocalDateToUtc(endDate.Date.AddDays(1))
            };
        }

        private static List<DashboardMonthBucket> BuildFallbackMonthBuckets(DateTime firstMonthLocal, int count)
        {
            return Enumerable.Range(0, count)
                .Select(index =>
                {
                    var monthStart = firstMonthLocal.AddMonths(index);
                    var nextMonthStart = monthStart.AddMonths(1);

                    return new DashboardMonthBucket
                    {
                        Label = monthStart.ToString("MMM yyyy", CultureInfo.InvariantCulture),
                        StartUtc = NepalLocalDateToUtc(monthStart),
                        EndUtc = NepalLocalDateToUtc(nextMonthStart)
                    };
                })
                .ToList();
        }

        private static bool MatchesActivityType(string? selectedType, string activityType)
        {
            if (string.IsNullOrWhiteSpace(selectedType) ||
                selectedType.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return selectedType.Equals(activityType, StringComparison.OrdinalIgnoreCase);
        }

        private static decimal CalculateAttendancePercentage(List<StudentAttendanceStatus> statuses)
        {
            if (statuses.Count == 0)
            {
                return 0m;
            }

            var weightedPresent = statuses.Count(x => x == StudentAttendanceStatus.Present) +
                                  statuses.Count(x => x == StudentAttendanceStatus.Late) +
                                  (statuses.Count(x => x == StudentAttendanceStatus.HalfDay) * 0.5m);

            return Math.Round((weightedPresent / statuses.Count) * 100, 2);
        }

        private static string GetExamTypeName(int examType)
        {
            return examType switch
            {
                1 => "First Terminal",
                2 => "Second Terminal",
                3 => "Third Terminal",
                4 => "Final Terminal",
                _ => $"Exam {examType}"
            };
        }

        private static string GetGenderName(int gender)
        {
            return gender switch
            {
                1 => "Male",
                2 => "Female",
                3 => "Other",
                _ => "Unknown"
            };
        }

        private sealed class DashboardDatePeriods
        {
            public string PeriodKey { get; set; } = "this-month";
            public string PeriodLabel { get; set; } = "This Month";
            public string FromDateNp { get; set; } = string.Empty;
            public string ToDateNp { get; set; } = string.Empty;
            public DateOnly TodayEn { get; set; }
            public DateTime TodayStartUtc { get; set; }
            public DateTime TomorrowStartUtc { get; set; }
            public DateTime RangeStartLocal { get; set; }
            public DateTime RangeEndLocalInclusive { get; set; }
            public DateTime RangeStartUtc { get; set; }
            public DateTime RangeEndUtc { get; set; }
            public List<DashboardMonthBucket> MonthBuckets { get; set; } = new();
        }

        private sealed class DashboardMonthBucket
        {
            public string Label { get; set; } = string.Empty;
            public DateTime StartUtc { get; set; }
            public DateTime EndUtc { get; set; }
        }

        private sealed class ActiveClassSectionRow
        {
            public Guid ClassSectionId { get; set; }
            public string ClassName { get; set; } = string.Empty;
            public string SectionName { get; set; } = string.Empty;
            public int OrderNumber { get; set; }
            public int TotalStudents { get; set; }
        }

        private sealed class AttendanceStatusRow
        {
            public Guid ClassSectionId { get; set; }
            public StudentAttendanceStatus Status { get; set; }
        }

        private sealed class FeeTotalRow
        {
            public Guid StudentEnrollmentId { get; set; }
            public string ClassName { get; set; } = string.Empty;
            public int ClassOrder { get; set; }
            public decimal NetAmount { get; set; }
            public decimal PaidAmount { get; set; }
            public decimal PendingAmount => Math.Max(NetAmount - PaidAmount, 0m);
        }
    }
}
