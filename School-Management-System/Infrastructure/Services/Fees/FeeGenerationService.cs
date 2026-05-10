using Application.Common.Interfaces;
using Application.Fees.Interfaces;
using Domain;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Fees
{
    public class FeeGenerationService : IFeeGenerationService
    {
        private readonly IApplicationDbContext _dbContext;

        public FeeGenerationService(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task EnsureFeesForEnrollmentAsync(Guid studentEnrollmentId, Guid academicYearId, CancellationToken cancellationToken)
        {
            var enrollment = await _dbContext.StudentEnrollments
                .Include(x => x.Student)
                .Include(x => x.ClassSection)
                .FirstOrDefaultAsync(
                    x => x.Id == studentEnrollmentId &&
                         x.AcademicYearId == academicYearId &&
                         x.IsActive &&
                         !x.IsDeleted &&
                         !x.Student.IsDeleted,
                    cancellationToken);

            if (enrollment == null)
            {
                return;
            }

            await CleanupLegacyAutoAddedNonRecurringFeesAsync(new List<Guid> { enrollment.Id }, academicYearId, cancellationToken);

            var feeStructures = await GetEligibleFeeStructuresAsync(enrollment.ClassSection.ClassId, academicYearId, cancellationToken);
            if (!feeStructures.Any())
            {
                return;
            }

            await EnsureFeesForEnrollmentsAsync(new List<StudentEnrollment> { enrollment }, feeStructures, cancellationToken);
        }

        public async Task SyncFeesForClassAsync(Guid classId, Guid academicYearId, CancellationToken cancellationToken)
        {
            var feeStructures = await GetEligibleFeeStructuresAsync(classId, academicYearId, cancellationToken);
            if (!feeStructures.Any())
            {
                return;
            }

            var enrollments = await _dbContext.StudentEnrollments
                .Include(x => x.Student)
                .Include(x => x.ClassSection)
                .Where(x => x.AcademicYearId == academicYearId &&
                            x.ClassSection.ClassId == classId &&
                            x.IsActive &&
                            !x.IsDeleted &&
                            !x.Student.IsDeleted)
                .ToListAsync(cancellationToken);

            if (!enrollments.Any())
            {
                return;
            }

            await CleanupLegacyAutoAddedNonRecurringFeesAsync(enrollments.Select(x => x.Id).ToList(), academicYearId, cancellationToken);
            await EnsureFeesForEnrollmentsAsync(enrollments, feeStructures, cancellationToken);
        }

        private async Task<List<FeeStructure>> GetEligibleFeeStructuresAsync(Guid classId, Guid academicYearId, CancellationToken cancellationToken)
        {
            return await _dbContext.FeeStructures
                .Include(x => x.FeeType)
                .Where(x => x.ClassId == classId &&
                            x.AcademicYearId == academicYearId &&
                            x.FeeType.IsRecurring &&
                            x.FeeType.Applicability != FeeApplicability.Manual)
                .ToListAsync(cancellationToken);
        }

        private async Task EnsureFeesForEnrollmentsAsync(
            IReadOnlyCollection<StudentEnrollment> enrollments,
            IReadOnlyCollection<FeeStructure> feeStructures,
            CancellationToken cancellationToken)
        {
            var enrollmentIds = enrollments.Select(x => x.Id).ToList();
            var feeStructureIds = feeStructures.Select(x => x.Id).ToList();

            var existingStudentFees = await _dbContext.StudentFees
                .Where(x => enrollmentIds.Contains(x.StudentEnrollmentId) && feeStructureIds.Contains(x.FeeStructureId))
                .ToListAsync(cancellationToken);

            var currentMonth = GetMonthStart(DateTime.UtcNow);
            var newStudentFees = new List<StudentFee>();

            foreach (var enrollment in enrollments)
            {
                var startMonth = GetMonthStart(enrollment.EnrollmentDate == default ? enrollment.CreatedDate : enrollment.EnrollmentDate);

                foreach (var feeStructure in feeStructures)
                {
                    if (!ShouldApplyFeeStructure(feeStructure, enrollment))
                    {
                        continue;
                    }

                    if (feeStructure.FeeType.IsRecurring)
                    {
                        var existingMonths = existingStudentFees
                            .Where(x => x.StudentEnrollmentId == enrollment.Id &&
                                        x.FeeStructureId == feeStructure.Id &&
                                        x.FeeMonth.HasValue)
                            .Select(x => GetMonthStart(x.FeeMonth!.Value))
                            .ToHashSet();

                        for (var month = startMonth; month <= currentMonth; month = month.AddMonths(1))
                        {
                            if (existingMonths.Contains(month))
                            {
                                continue;
                            }

                            newStudentFees.Add(new StudentFee
                            {
                                StudentEnrollmentId = enrollment.Id,
                                FeeStructureId = feeStructure.Id,
                                Amount = feeStructure.Amount,
                                FeeMonth = month,
                                IsPaid = false,
                                Origin = StudentFeeOrigin.AutoRecurring,
                                CreatedDate = DateTime.UtcNow
                            });
                        }
                    }
                }
            }

            if (!newStudentFees.Any())
            {
                return;
            }

            await _dbContext.StudentFees.AddRangeAsync(newStudentFees, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task CleanupLegacyAutoAddedNonRecurringFeesAsync(
            IReadOnlyCollection<Guid> enrollmentIds,
            Guid academicYearId,
            CancellationToken cancellationToken)
        {
            if (!enrollmentIds.Any())
            {
                return;
            }

            var removableFees = await _dbContext.StudentFees
                .Include(x => x.Payments)
                .Include(x => x.FeeAdjustments)
                .Include(x => x.StudentEnrollment)
                .Include(x => x.FeeStructure)
                    .ThenInclude(x => x.FeeType)
                .Where(x => enrollmentIds.Contains(x.StudentEnrollmentId) &&
                            x.StudentEnrollment.AcademicYearId == academicYearId &&
                            !x.FeeStructure.FeeType.IsRecurring &&
                            x.Origin == StudentFeeOrigin.Unknown &&
                            !x.FeeMonth.HasValue &&
                            !x.Payments.Any() &&
                            !x.FeeAdjustments.Any())
                .ToListAsync(cancellationToken);

            if (!removableFees.Any())
            {
                return;
            }

            _dbContext.StudentFees.RemoveRange(removableFees);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private static bool ShouldApplyFeeStructure(FeeStructure feeStructure, StudentEnrollment enrollment)
        {
            return feeStructure.FeeType.Applicability switch
            {
                FeeApplicability.BusConditional => enrollment.IsBusRequired,
                FeeApplicability.Manual => false,
                _ => true
            };
        }

        private static DateTime GetMonthStart(DateTime value)
        {
            var normalized = value.Kind == DateTimeKind.Utc ? value : DateTime.SpecifyKind(value, DateTimeKind.Utc);
            return new DateTime(normalized.Year, normalized.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        }
    }
}
