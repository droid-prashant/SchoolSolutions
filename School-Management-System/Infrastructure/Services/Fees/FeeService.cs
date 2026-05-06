using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Fees.Dtos;
using Application.Fees.Interfaces;
using Application.Fees.ViewModel;
using Domain;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Fees
{
    public class FeeService : IFeeService
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly UserResolver _userResolver;
        public FeeService(IApplicationDbContext dbContext, UserResolver userResolver)
        {
            _dbContext = dbContext;
            _userResolver = userResolver;
        }

        private Guid GetCurrentAcademicYearId()
        {
            return _userResolver.GetAcademicYearGuidOrThrow();
        }

        private static DateTime GetMonthStart(DateTime value)
        {
            var normalized = value.Kind == DateTimeKind.Utc ? value : DateTime.SpecifyKind(value, DateTimeKind.Utc);
            return new DateTime(normalized.Year, normalized.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        }

        private static decimal GetDiscountTotal(StudentFee studentFee)
        {
            return studentFee.FeeAdjustments.Sum(x => x.DiscountAmount);
        }

        private static decimal GetFineTotal(StudentFee studentFee)
        {
            return studentFee.FeeAdjustments.Sum(x => x.FineAmount);
        }

        private static decimal GetNetAmount(StudentFee studentFee)
        {
            return studentFee.Amount - GetDiscountTotal(studentFee) + GetFineTotal(studentFee);
        }

        private static decimal GetPaidAmount(StudentFee studentFee)
        {
            return studentFee.Payments.Sum(x => x.AmountPaid);
        }

        private static decimal GetPendingAmount(StudentFee studentFee)
        {
            return Math.Max(GetNetAmount(studentFee) - GetPaidAmount(studentFee), 0);
        }

        private async Task SyncFeesForClassAsync(Guid classId, CancellationToken cancellationToken)
        {
            var currentAcademicYearId = GetCurrentAcademicYearId();
            var feeStructures = await _dbContext.FeeStructures
                .Include(x => x.FeeType)
                .Where(x => x.ClassId == classId && x.AcademicYearId == currentAcademicYearId)
                .ToListAsync(cancellationToken);

            if (!feeStructures.Any())
            {
                return;
            }

            var enrollments = await _dbContext.StudentEnrollments
                .Include(x => x.Student)
                .Include(x => x.ClassSection)
                .Where(x => x.AcademicYearId == currentAcademicYearId &&
                            x.ClassSection.ClassId == classId &&
                            x.IsActive &&
                            !x.IsDeleted &&
                            !x.Student.IsDeleted)
                .ToListAsync(cancellationToken);

            if (!enrollments.Any())
            {
                return;
            }

            var enrollmentIds = enrollments.Select(x => x.Id).ToList();
            var feeStructureIds = feeStructures.Select(x => x.Id).ToList();
            var existingStudentFees = await _dbContext.StudentFees
                .Where(x => enrollmentIds.Contains(x.StudentEnrollmentId) && feeStructureIds.Contains(x.FeeStructureId))
                .ToListAsync(cancellationToken);

            var currentMonth = GetMonthStart(DateTime.UtcNow);
            var newStudentFees = new List<StudentFee>();

            foreach (var enrollment in enrollments)
            {
                var startMonth = GetMonthStart(enrollment.CreatedDate);

                foreach (var feeStructure in feeStructures)
                {
                    if (feeStructure.FeeType.IsRecurring)
                    {
                        var existingMonths = existingStudentFees
                            .Where(x => x.StudentEnrollmentId == enrollment.Id && x.FeeStructureId == feeStructure.Id && x.FeeMonth.HasValue)
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
                                CreatedDate = DateTime.UtcNow
                            });
                        }
                    }
                    else
                    {
                        var alreadyExists = existingStudentFees.Any(x =>
                            x.StudentEnrollmentId == enrollment.Id &&
                            x.FeeStructureId == feeStructure.Id &&
                            !x.FeeMonth.HasValue);

                        if (alreadyExists)
                        {
                            continue;
                        }

                        newStudentFees.Add(new StudentFee
                        {
                            StudentEnrollmentId = enrollment.Id,
                            FeeStructureId = feeStructure.Id,
                            Amount = feeStructure.Amount,
                            FeeMonth = null,
                            IsPaid = false,
                            CreatedDate = DateTime.UtcNow
                        });
                    }
                }
            }

            if (newStudentFees.Any())
            {
                await _dbContext.StudentFees.AddRangeAsync(newStudentFees, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<List<FeeTypeViewModel>> GetFeeType(CancellationToken cancellationToken)
        {
            var result = await _dbContext.FeeTypes.Select(x => new FeeTypeViewModel
            {
                Id = x.Id.ToString(),
                Name = x.Name,
                IsRecurring = x.IsRecurring,
                Frequency = x.Frequency,
            }).ToListAsync(cancellationToken);
            return result;
        }

        public async Task<List<FeeStructureViewModel>> GetFeeStructure(string classId, CancellationToken cancellationToken)
        {
            var currentAcademicYearId = GetCurrentAcademicYearId();
            var result = await _dbContext.FeeStructures
                                                   .Where(x => x.ClassId == Guid.Parse(classId) && x.AcademicYearId == currentAcademicYearId)
                                                   .Select(x => new FeeStructureViewModel
                                                   {
                                                       Id = x.Id,
                                                       AcademicYearId = x.AcademicYearId.ToString(),
                                                       AcademicYearName = x.AcademicYear.YearName,
                                                       FeeTypeId = x.FeeTypeId.ToString(),
                                                       FeeType = x.FeeType.Name,
                                                       ClassId = x.ClassId.ToString(),
                                                       Class = x.ClassRoom.Name,
                                                       Amount = x.Amount,
                                                       Description = x.Description
                                                   }).ToListAsync(cancellationToken);
            return result;
        }

        public async Task<FeeReportViewModel?> GetFeeReport(string classSectionId, CancellationToken cancellationToken)
        {
            var currentAcademicYearId = GetCurrentAcademicYearId();
            var parsedClassSectionId = Guid.Parse(classSectionId);
            var classSection = await _dbContext.ClassSections
                .Include(cs => cs.ClassRoom)
                .Include(cs => cs.Section)
                .FirstOrDefaultAsync(cs => cs.Id == parsedClassSectionId, cancellationToken);

            if (classSection == null)
            {
                return null;
            }

            await SyncFeesForClassAsync(classSection.ClassId, cancellationToken);

            var enrollments = await _dbContext.StudentEnrollments
                .Include(se => se.Student)
                .Include(se => se.ClassSection)
                    .ThenInclude(cs => cs.ClassRoom)
                .Include(se => se.ClassSection)
                    .ThenInclude(cs => cs.Section)
                .Include(se => se.AcademicYear)
                .Where(se => se.ClassSectionId == parsedClassSectionId &&
                             se.AcademicYearId == currentAcademicYearId &&
                             se.IsActive &&
                             !se.IsDeleted &&
                             !se.Student.IsDeleted)
                .OrderBy(se => se.RollNumber ?? int.MaxValue)
                .ThenBy(se => se.Student.FirstName)
                .ThenBy(se => se.Student.LastName)
                .ToListAsync(cancellationToken);

            if (!enrollments.Any())
            {
                //var classSection = await _dbContext.ClassSections
                //    .Include(cs => cs.ClassRoom)
                //    .Include(cs => cs.Section)
                //    .FirstOrDefaultAsync(cs => cs.Id == parsedClassSectionId, cancellationToken);

                var academicYearName = await _dbContext.AcademicYears
                    .Where(x => x.Id == currentAcademicYearId)
                    .Select(x => x.YearName)
                    .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;

                return new FeeReportViewModel
                {
                    AcademicYearName = academicYearName,
                    ClassName = classSection.ClassRoom.Name,
                    SectionName = classSection.Section.Name
                };
            }

            var enrollmentIds = enrollments.Select(x => x.Id).ToList();
            var studentIds = enrollments.Select(x => x.StudentId).Distinct().ToList();
            var studentFees = await _dbContext.StudentFees
                .Include(sf => sf.Payments)
                .Include(sf => sf.FeeAdjustments)
                .Where(sf => enrollmentIds.Contains(sf.StudentEnrollmentId))
                .ToListAsync(cancellationToken);
            var previousYearFeeGroups = await _dbContext.StudentFees
                .Include(sf => sf.Payments)
                .Include(sf => sf.FeeAdjustments)
                .Include(sf => sf.StudentEnrollment)
                    .ThenInclude(se => se.AcademicYear)
                .Where(sf => studentIds.Contains(sf.StudentEnrollment.StudentId) && sf.StudentEnrollment.AcademicYearId != currentAcademicYearId)
                .GroupBy(sf => sf.StudentEnrollment.StudentId)
                .Select(g => new
                {
                    StudentId = g.Key,
                    PendingAmount = g.Sum(x => x.Amount - x.FeeAdjustments.Sum(a => a.DiscountAmount) + x.FeeAdjustments.Sum(a => a.FineAmount)) -
                                    g.SelectMany(x => x.Payments).Sum(p => p.AmountPaid)
                })
                .ToListAsync(cancellationToken);

            var students = enrollments.Select(enrollment =>
            {
                var fees = studentFees.Where(sf => sf.StudentEnrollmentId == enrollment.Id).ToList();
                var totalFees = fees.Sum(sf => sf.Amount);
                var totalDiscount = fees.Sum(GetDiscountTotal);
                var totalFine = fees.Sum(GetFineTotal);
                var netFees = fees.Sum(GetNetAmount);
                var totalPaid = fees.Sum(GetPaidAmount);
                var totalPending = fees.Sum(GetPendingAmount);
                var previousYearPending = previousYearFeeGroups
                    .Where(x => x.StudentId == enrollment.StudentId)
                    .Select(x => x.PendingAmount)
                    .FirstOrDefault();

                return new FeeReportRowViewModel
                {
                    StudentEnrollmentId = enrollment.Id,
                    StudentName = $"{enrollment.Student.FirstName} {enrollment.Student.LastName}".Trim(),
                    RollNumber = enrollment.RollNumber,
                    TotalFees = totalFees,
                    TotalDiscount = totalDiscount,
                    TotalFine = totalFine,
                    NetFees = netFees,
                    TotalPaid = totalPaid,
                    TotalPending = totalPending,
                    PreviousYearPending = previousYearPending,
                    GrandTotalPending = totalPending + previousYearPending
                };
            }).ToList();

            var firstEnrollment = enrollments.First();
            return new FeeReportViewModel
            {
                AcademicYearName = firstEnrollment.AcademicYear.YearName,
                ClassName = firstEnrollment.ClassSection.ClassRoom.Name,
                SectionName = firstEnrollment.ClassSection.Section.Name,
                TotalStudents = students.Count,
                TotalFees = students.Sum(x => x.TotalFees),
                TotalDiscount = students.Sum(x => x.TotalDiscount),
                TotalFine = students.Sum(x => x.TotalFine),
                NetFees = students.Sum(x => x.NetFees),
                TotalPaid = students.Sum(x => x.TotalPaid),
                TotalPending = students.Sum(x => x.TotalPending),
                TotalPreviousYearPending = students.Sum(x => x.PreviousYearPending),
                GrandTotalPending = students.Sum(x => x.GrandTotalPending),
                Students = students
            };
        }

        public async Task AddFeeType(FeeTypeDto feeTypeDto, CancellationToken cancellationToken)
        {
            var feeType = new FeeType
            {
                Name = feeTypeDto.Name,
                IsRecurring = feeTypeDto.IsRecurring,
                Frequency = feeTypeDto.Frequency,
                CreatedDate = DateTime.UtcNow
            };
            await _dbContext.FeeTypes.AddAsync(feeType);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateFeeType(FeeTypeDto feeTypeDto, string feeTypeId, CancellationToken cancellationToken)
        {
            var existingFeeType = await _dbContext.FeeTypes.FirstOrDefaultAsync(x => x.Id == Guid.Parse(feeTypeId));
            if (existingFeeType != null)
            {
                existingFeeType.Name = feeTypeDto.Name;
                existingFeeType.Frequency = feeTypeDto.Frequency;
                existingFeeType.IsRecurring = feeTypeDto.IsRecurring;
                existingFeeType.ModifiedDate = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task AddFeeStructure(FeeStructureDto feeStructureDto, CancellationToken cancellationToken)
        {
            var currentAcademicYearId = GetCurrentAcademicYearId();
            if (feeStructureDto.AcademicYearId != Guid.Empty && feeStructureDto.AcademicYearId != currentAcademicYearId)
            {
                throw new Exception("Fee structure must be created for the current academic year.");
            }

            var isDuplicate = await _dbContext.FeeStructures.AnyAsync(x =>
                x.AcademicYearId == currentAcademicYearId &&
                x.ClassId == feeStructureDto.ClassId &&
                x.FeeTypeId == feeStructureDto.FeeTypeId, cancellationToken);

            if (isDuplicate)
            {
                throw new Exception("Fee structure already exists for this fee type, class, and academic year.");
            }

            var feeStructure = new FeeStructure
            {
                AcademicYearId = currentAcademicYearId,
                FeeTypeId = feeStructureDto.FeeTypeId,
                ClassId = feeStructureDto.ClassId,
                Amount = feeStructureDto.Amount,
                Description = feeStructureDto.Description,
                CreatedDate = DateTime.UtcNow
            };
            await _dbContext.FeeStructures.AddAsync(feeStructure);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await SyncFeesForClassAsync(feeStructure.ClassId, cancellationToken);
        }

        public async Task UpdateFeeSctucture(FeeStructureDto feeStructureDto, string feeStructureId, CancellationToken cancellationToken)
        {
            var currentAcademicYearId = GetCurrentAcademicYearId();
            var existingFeeStructure = _dbContext.FeeStructures
                .Where(x => x.Id == Guid.Parse(feeStructureId) && x.AcademicYearId == currentAcademicYearId)
                .FirstOrDefault();

            if (existingFeeStructure != null)
            {
                var duplicateExists = await _dbContext.FeeStructures.AnyAsync(x =>
                    x.Id != existingFeeStructure.Id &&
                    x.AcademicYearId == currentAcademicYearId &&
                    x.ClassId == feeStructureDto.ClassId &&
                    x.FeeTypeId == feeStructureDto.FeeTypeId, cancellationToken);

                if (duplicateExists)
                {
                    throw new Exception("Another fee structure already exists for this fee type, class, and academic year.");
                }

                existingFeeStructure.AcademicYearId = currentAcademicYearId;
                existingFeeStructure.FeeTypeId = feeStructureDto.FeeTypeId;
                existingFeeStructure.ClassId = feeStructureDto.ClassId;
                existingFeeStructure.Amount = feeStructureDto.Amount;
                existingFeeStructure.Description = feeStructureDto.Description;
                existingFeeStructure.ModifiedDate = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync(cancellationToken);
                await SyncFeesForClassAsync(existingFeeStructure.ClassId, cancellationToken);
            }
        }

        public async Task<StudentFeeSummaryViewModel> GetStudentFeeSummary(string studentEnrollmentIdGuid, string classSectionId, CancellationToken cancellationToken)
        {
            var currentAcademicYearId = GetCurrentAcademicYearId();
            var studentEnrollment = await _dbContext.StudentEnrollments.Include(s => s.Student)
                                                             .Include(s => s.ClassSection)
                                                             .ThenInclude(cs => cs.ClassRoom)
                                                             .Include(x => x.ClassSection.Section)
                                                             .Include(x => x.AcademicYear)
                                                             .FirstOrDefaultAsync(s => s.Id == Guid.Parse(studentEnrollmentIdGuid) &&
                                                                                      s.ClassSectionId == Guid.Parse(classSectionId) &&
                                                                                      s.AcademicYearId == currentAcademicYearId, cancellationToken);

            if (studentEnrollment == null)
                return null;

            await SyncFeesForClassAsync(studentEnrollment.ClassSection.ClassId, cancellationToken);

            var studentFees = await _dbContext.StudentFees.Include(sf => sf.FeeStructure)
                                                          .ThenInclude(fs => fs.FeeType)
                                                          .Include(sf => sf.Payments)
                                                          .Include(sf => sf.FeeAdjustments)
                                                          .Where(sf => sf.StudentEnrollmentId == studentEnrollment.Id)
                                                          .ToListAsync(cancellationToken);

            var feeDetails = studentFees.GroupBy(f => new
            {
                Id = f.Id,
                ClassName = f.FeeStructure.ClassRoom.Name,
                FeeType = f.FeeStructure.FeeType.Name,
                f.FeeMonth
            })
                                            .Select(g => new StudentFeeDetailViewModel
                                            {
                                                Id = g.Key.Id.ToString(),
                                                AcademicYearName = studentEnrollment.AcademicYear.YearName,
                                                ClassName = g.Key.ClassName,
                                                FeeType = g.Key.FeeType,
                                                FeeMonth = g.Key.FeeMonth,
                                                DiscountAmount = g.Sum(x => x.FeeAdjustments.Sum(a => a.DiscountAmount)),
                                                FineAmount = g.Sum(x => x.FeeAdjustments.Sum(a => a.FineAmount)),
                                                TotalAmount = g.Sum(x => x.Amount),
                                                NetAmount = g.Sum(x => x.Amount - x.FeeAdjustments.Sum(a => a.DiscountAmount) + x.FeeAdjustments.Sum(a => a.FineAmount)),
                                                PaidAmount = g.SelectMany(x => x.Payments).Sum(p => p.AmountPaid),
                                                PendingAmount = Math.Max(g.Sum(x => x.Amount - x.FeeAdjustments.Sum(a => a.DiscountAmount) + x.FeeAdjustments.Sum(a => a.FineAmount)) - g.SelectMany(x => x.Payments).Sum(p => p.AmountPaid), 0),
                                                IsPaid = g.Sum(x => x.Amount - x.FeeAdjustments.Sum(a => a.DiscountAmount) + x.FeeAdjustments.Sum(a => a.FineAmount)) <= g.SelectMany(x => x.Payments).Sum(p => p.AmountPaid)
                                            }).OrderBy(x => x.FeeType == "Monthy Fee").ThenBy(x => x.FeeMonth)
                                            .ToList();

            var previousYearFeeDetails = await _dbContext.StudentFees
                .Include(sf => sf.FeeStructure)
                    .ThenInclude(fs => fs.FeeType)
                .Include(sf => sf.Payments)
                .Include(sf => sf.FeeAdjustments)
                .Include(sf => sf.StudentEnrollment)
                    .ThenInclude(se => se.AcademicYear)
                .Include(sf => sf.StudentEnrollment)
                    .ThenInclude(se => se.ClassSection)
                        .ThenInclude(cs => cs.ClassRoom)
                .Where(sf => sf.StudentEnrollment.StudentId == studentEnrollment.StudentId &&
                             sf.StudentEnrollment.AcademicYearId != currentAcademicYearId)
                .Select(sf => new StudentFeeDetailViewModel
                {
                    Id = sf.Id.ToString(),
                    AcademicYearName = sf.StudentEnrollment.AcademicYear.YearName,
                    ClassName = sf.StudentEnrollment.ClassSection.ClassRoom.Name,
                    FeeType = sf.FeeStructure.FeeType.Name,
                    FeeMonth = sf.FeeMonth,
                    DiscountAmount = sf.FeeAdjustments.Sum(a => a.DiscountAmount),
                    FineAmount = sf.FeeAdjustments.Sum(a => a.FineAmount),
                    TotalAmount = sf.Amount,
                    NetAmount = sf.Amount - sf.FeeAdjustments.Sum(a => a.DiscountAmount) + sf.FeeAdjustments.Sum(a => a.FineAmount),
                    PaidAmount = sf.Payments.Sum(p => p.AmountPaid),
                    PendingAmount = Math.Max((sf.Amount - sf.FeeAdjustments.Sum(a => a.DiscountAmount) + sf.FeeAdjustments.Sum(a => a.FineAmount)) - sf.Payments.Sum(p => p.AmountPaid), 0),
                    IsPaid = (sf.Amount - sf.FeeAdjustments.Sum(a => a.DiscountAmount) + sf.FeeAdjustments.Sum(a => a.FineAmount)) <= sf.Payments.Sum(p => p.AmountPaid)
                })
                .Where(x => x.PendingAmount > 0)
                .OrderBy(x => x.AcademicYearName)
                .ThenBy(x => x.FeeType == "Monthy Fee")
                .ThenBy(x => x.FeeMonth)
                .ToListAsync(cancellationToken);

            var previousYearFees = await _dbContext.StudentFees
                .Include(sf => sf.Payments)
                .Include(sf => sf.FeeAdjustments)
                .Include(sf => sf.StudentEnrollment)
                    .ThenInclude(se => se.AcademicYear)
                .Where(sf => sf.StudentEnrollment.StudentId == studentEnrollment.StudentId &&
                             sf.StudentEnrollment.AcademicYearId != currentAcademicYearId)
                .GroupBy(sf => new
                {
                    sf.StudentEnrollment.AcademicYearId,
                    sf.StudentEnrollment.AcademicYear.YearName
                })
                .Select(g => new PreviousYearDueViewModel
                {
                    AcademicYearName = g.Key.YearName,
                    PendingAmount = g.Sum(x => x.Amount - x.FeeAdjustments.Sum(a => a.DiscountAmount) + x.FeeAdjustments.Sum(a => a.FineAmount)) -
                                    g.SelectMany(x => x.Payments).Sum(p => p.AmountPaid)
                })
                .Where(x => x.PendingAmount > 0)
                .OrderBy(x => x.AcademicYearName)
                .ToListAsync(cancellationToken);

            var previousYearPending = previousYearFees.Sum(x => x.PendingAmount);
            var totalFees = feeDetails.Sum(f => f.TotalAmount);
            var totalDiscount = feeDetails.Sum(f => f.DiscountAmount);
            var totalFine = feeDetails.Sum(f => f.FineAmount);
            var netFees = feeDetails.Sum(f => f.NetAmount);
            var totalPaid = feeDetails.Sum(f => f.PaidAmount);
            var totalPending = feeDetails.Sum(f => f.PendingAmount);

            return new StudentFeeSummaryViewModel
            {
                StudentEnrollmentId = studentEnrollment.Id,
                AcademicYearName = studentEnrollment.AcademicYear.YearName,
                StudentName = $"{studentEnrollment.Student.FirstName} {studentEnrollment.Student.LastName}",
                ClassName = studentEnrollment.ClassSection.ClassRoom.Name,
                SectionName = studentEnrollment.ClassSection.Section.Name,
                TotalFees = totalFees,
                TotalDiscount = totalDiscount,
                TotalFine = totalFine,
                NetFees = netFees,
                TotalPaid = totalPaid,
                TotalPending = totalPending,
                PreviousYearPending = previousYearPending,
                GrandTotalPending = totalPending + previousYearPending,
                FeeDetails = feeDetails,
                PreviousYearDues = previousYearFees,
                PreviousYearFeeDetails = previousYearFeeDetails
            };
        }

        public async Task ApplyFeeAdjustmentAsync(FeeAdjustmentDto feeAdjustmentDto, CancellationToken cancellationToken)
        {
            if (feeAdjustmentDto.DiscountAmount < 0 || feeAdjustmentDto.FineAmount < 0)
            {
                throw new Exception("Adjustment amounts cannot be negative.");
            }

            if (feeAdjustmentDto.DiscountAmount == 0 && feeAdjustmentDto.FineAmount == 0)
            {
                throw new Exception("Enter a discount or fine amount.");
            }

            if (string.IsNullOrWhiteSpace(feeAdjustmentDto.Reason))
            {
                throw new Exception("Adjustment reason is required.");
            }

            var currentAcademicYearId = GetCurrentAcademicYearId();
            var studentFee = await _dbContext.StudentFees
                .Include(sf => sf.StudentEnrollment)
                .Include(sf => sf.Payments)
                .Include(sf => sf.FeeAdjustments)
                .FirstOrDefaultAsync(sf => sf.Id == feeAdjustmentDto.StudentFeeId && sf.StudentEnrollment.AcademicYearId == currentAcademicYearId, cancellationToken);

            if (studentFee == null)
            {
                throw new Exception("Fee record not found.");
            }

            var existingDiscount = GetDiscountTotal(studentFee);
            var existingFine = GetFineTotal(studentFee);
            var maxDiscount = studentFee.Amount + existingFine - existingDiscount;

            if (feeAdjustmentDto.DiscountAmount > maxDiscount)
            {
                throw new Exception("Discount amount cannot exceed the net fee amount.");
            }

            var adjustment = new FeeAdjustment
            {
                StudentFeeId = studentFee.Id,
                DiscountAmount = feeAdjustmentDto.DiscountAmount,
                FineAmount = feeAdjustmentDto.FineAmount,
                Reason = feeAdjustmentDto.Reason
            };

            await _dbContext.FeeAdjustments.AddAsync(adjustment, cancellationToken);

            var updatedNetAmount = GetNetAmount(studentFee) - feeAdjustmentDto.DiscountAmount + feeAdjustmentDto.FineAmount;
            studentFee.IsPaid = GetPaidAmount(studentFee) >= updatedNetAmount;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task EnsureMissingMonthlyFeesAsync(string studentEnrollmentIdGuid, CancellationToken cancellationToken)
        {
            var currentAcademicYearId = GetCurrentAcademicYearId();
            var studentEnrollment = await _dbContext.StudentEnrollments
                                                    .Include(s => s.ClassSection)
                                                    .FirstOrDefaultAsync(s => s.Id == Guid.Parse(studentEnrollmentIdGuid) &&
                                                                              s.AcademicYearId == currentAcademicYearId, cancellationToken);

            if (studentEnrollment == null)
            {
                return;
            }

            await SyncFeesForClassAsync(studentEnrollment.ClassSection.ClassId, cancellationToken);
        }


        public async Task PayStudentFeeAsync(Guid studentFeeId, Guid currentStudentEnrollmentId, decimal amount, string paymentMode, CancellationToken cancellationToken)
        {
            if (amount <= 0)
            {
                throw new Exception("Payment amount must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(paymentMode))
            {
                throw new Exception("Payment mode is required.");
            }

            var currentAcademicYearId = GetCurrentAcademicYearId();
            var currentEnrollment = await _dbContext.StudentEnrollments
                .FirstOrDefaultAsync(x => x.Id == currentStudentEnrollmentId && x.AcademicYearId == currentAcademicYearId, cancellationToken);

            if (currentEnrollment == null)
            {
                throw new Exception("The selected student does not belong to the current academic year.");
            }

            var studentFee = await _dbContext.StudentFees
                .Include(sf => sf.StudentEnrollment)
                .Include(sf => sf.Payments)
                .Include(sf => sf.FeeAdjustments)
                .FirstOrDefaultAsync(sf => sf.Id == studentFeeId, cancellationToken);

            if (studentFee == null) throw new Exception("Student fee not found");

            if (studentFee.StudentEnrollment.StudentId != currentEnrollment.StudentId)
            {
                throw new Exception("The selected fee does not belong to the current student.");
            }

            var pendingAmount = GetPendingAmount(studentFee);

            if (pendingAmount <= 0)
            {
                throw new Exception("This fee has already been fully paid.");
            }

            if (amount > pendingAmount)
            {
                throw new Exception("Payment amount cannot exceed the pending amount.");
            }

            studentFee.Payments.Add(new Payment
            {
                StudentFeeId = studentFee.Id,
                AmountPaid = amount,
                PaymentDate = DateTime.UtcNow,
                Method = paymentMode
            });

            if (studentFee.Payments.Sum(p => p.AmountPaid) >= GetNetAmount(studentFee))
                studentFee.IsPaid = true;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

    }
}
