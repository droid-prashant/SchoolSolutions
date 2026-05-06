using Application.Common.Interfaces;
using Application.Dashboard.Interfaces;
using Application.Dashboard.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Dashboard
{
    public class DasboardService : IDashboardService
    {
        private readonly IApplicationDbContext _context;
        public DasboardService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetCoursesCount(CancellationToken cancellationToken)
        {
            int coursesCount = await _context.Courses.CountAsync(x => !x.IsDeleted, cancellationToken);
            return coursesCount;
        }

        public async Task<int> GetStudentCount(CancellationToken cancellationToken)
        {
            int studentCount = await _context.StudentEnrollments.CountAsync(x => !x.IsDeleted && x.IsActive && !x.Student.IsDeleted, cancellationToken);
            return studentCount;
        }

        public async Task<int> GetTeachersCount(CancellationToken cancellationToken)
        {
            return await _context.Teachers.CountAsync(x => !x.IsDeleted && x.IsActive, cancellationToken);
        }

        public async Task<DashboardSummaryViewModel> GetDashboardSummary(CancellationToken cancellationToken)
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            var feeTotals = await _context.StudentFees
                .Where(x => !x.IsDeleted)
                .Select(x => new
                {
                    StudentEnrollmentId = x.StudentEnrollmentId,
                    NetAmount = x.Amount - x.FeeAdjustments.Sum(a => a.DiscountAmount) + x.FeeAdjustments.Sum(a => a.FineAmount),
                    PaidAmount = x.Payments.Sum(p => p.AmountPaid)
                })
                .ToListAsync(cancellationToken);

            return new DashboardSummaryViewModel
            {
                StudentsCount = await GetStudentCount(cancellationToken),
                TeachersCount = await GetTeachersCount(cancellationToken),
                CoursesCount = await GetCoursesCount(cancellationToken),
                FeePendingOverall = feeTotals.Sum(x => Math.Max(x.NetAmount - x.PaidAmount, 0m)),
                FeeCollectedOverall = feeTotals.Sum(x => x.PaidAmount),
                FeeCollectedToday = await _context.Payments
                    .Where(x => !x.IsDeleted && x.PaymentDate >= today && x.PaymentDate < tomorrow)
                    .SumAsync(x => x.AmountPaid, cancellationToken),
                PendingFeeStudents = feeTotals
                    .Where(x => x.NetAmount > x.PaidAmount)
                    .Select(x => x.StudentEnrollmentId)
                    .Distinct()
                    .Count()
            };
        }

        public async Task<List<StudentsByClassViewModel>> GetStudentsByClass(CancellationToken cancellationToken)
        {
            var result = await _context.StudentEnrollments
                                 .Include(x => x.ClassSection)
                                 .ThenInclude(x => x.ClassRoom)
                                 .Where(x => x.IsActive && !x.IsDeleted && !x.Student.IsDeleted)
                                 .GroupBy(x => x.ClassSection.ClassRoom.Name)
                                 .Select(g => new StudentsByClassViewModel
                                 {
                                     ClassRoom = g.Key,
                                     StudentsCountBySections = g.Select(x => new StudentCountBySection
                                     {
                                         SectionName = x.ClassSection.Section.Name,
                                         StudentCount = x.ClassSection.StudentEnrollments.Count(e => e.IsActive && !e.IsDeleted && !e.Student.IsDeleted)
                                     }).ToList()
                                 }).ToListAsync(cancellationToken);

            return result;
        }
    }
}
