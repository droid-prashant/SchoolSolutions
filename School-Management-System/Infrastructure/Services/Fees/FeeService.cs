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
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Fees
{
    public class FeeService : IFeeService
    {
        private readonly IApplicationDbContext _dbContext;
        public FeeService(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<FeeTypeViewModel>> GetFeeType(CancellationToken cancellationToken)
        {
            var result = await _dbContext.FeeTypes.Select(x => new FeeTypeViewModel
            {
                Id = x.Id.ToString(),
                Name = x.Name,
                IsRecurring = x.IsRecurring == true ? "Yes" : "No",
                Frequency = x.Frequency,
            }).ToListAsync(cancellationToken);
            return result;
        }

        public async Task<List<FeeStructureViewModel>> GetFeeStructure(string classId, CancellationToken cancellationToken)
        {
            var result = await _dbContext.FeeStructures.Where(x => x.ClassId == Guid.Parse(classId))
                                                   .Select(x => new FeeStructureViewModel
                                                   {
                                                       Id = x.Id,
                                                       FeeTypeId = x.FeeTypeId.ToString(),
                                                       FeeType = x.FeeType.Name,
                                                       ClassId = x.ClassId.ToString(),
                                                       Class = x.ClassRoom.Name,
                                                       Amount = x.Amount,
                                                       Description = x.Description
                                                   }).ToListAsync(cancellationToken);
            return result;
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
            var feeStructure = new FeeStructure
            {
                FeeTypeId = feeStructureDto.FeeTypeId,
                ClassId = feeStructureDto.ClassId,
                Amount = feeStructureDto.Amount,
                Description = feeStructureDto.Description,
                CreatedDate = DateTime.UtcNow
            };
            await _dbContext.FeeStructures.AddAsync(feeStructure);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateFeeSctucture(FeeStructureDto feeStructureDto, string feeStructureId, CancellationToken cancellationToken)
        {
            var existingFeeStructure = _dbContext.FeeStructures.Where(x => x.Id == Guid.Parse(feeStructureId)).FirstOrDefault();
            if (existingFeeStructure != null)
            {
                existingFeeStructure.FeeTypeId = feeStructureDto.FeeTypeId;
                existingFeeStructure.ClassId = feeStructureDto.ClassId;
                existingFeeStructure.Amount = feeStructureDto.Amount;
                existingFeeStructure.Description = feeStructureDto.Description;
                existingFeeStructure.ModifiedDate = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<StudentFeeSummaryViewModel> GetStudentFeeSummary(string studentEnrollmentIdGuid, string classSectionId, CancellationToken cancellationToken)
        {
            var studentEnrollment = await _dbContext.StudentEnrollments.Include(s => s.Student)
                                                             .Include(s => s.ClassSection)
                                                             .ThenInclude(cs => cs.ClassRoom)
                                                             .Include(x => x.ClassSection.Section)
                                                             .FirstOrDefaultAsync(s => s.Id == Guid.Parse(studentEnrollmentIdGuid) && s.ClassSectionId == Guid.Parse(classSectionId), cancellationToken);

            if (studentEnrollment == null)
                return null;


            var studentFees = await _dbContext.StudentFees.Include(sf => sf.FeeStructure)
                                                          .ThenInclude(fs => fs.FeeType)
                                                          .Include(sf => sf.Payments)
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
                                                ClassName = g.Key.ClassName,
                                                FeeType = g.Key.FeeType,
                                                FeeMonth = g.Key.FeeMonth,
                                                TotalAmount = g.Sum(x => x.Amount),
                                                PaidAmount = g.SelectMany(x => x.Payments).Sum(p => p.AmountPaid),
                                                PendingAmount = g.Sum(x => x.Amount) - g.SelectMany(x => x.Payments).Sum(p => p.AmountPaid),
                                                IsPaid = g.Sum(x => x.Amount) <= g.SelectMany(x => x.Payments).Sum(p => p.AmountPaid)
                                            }).OrderBy(x => x.FeeType == "Monthy Fee").ThenBy(x => x.FeeMonth)
                                            .ToList();

            return new StudentFeeSummaryViewModel
            {
                StudentEnrollmentId = studentEnrollment.Id,
                StudentName = $"{studentEnrollment.Student.FirstName} {studentEnrollment.Student.LastName}",
                ClassName = studentEnrollment.ClassSection.ClassRoom.Name,
                SectionName = studentEnrollment.ClassSection.Section.Name,
                TotalFees = feeDetails.Sum(f => f.TotalAmount),
                TotalPaid = feeDetails.Sum(f => f.PaidAmount),
                TotalPending = feeDetails.Sum(f => f.PendingAmount),
                FeeDetails = feeDetails
            };
        }

        public async Task EnsureMissingMonthlyFeesAsync(string studentEnrollmentIdGuid, CancellationToken cancellationToken)
        {
            try
            {
                var studentEnrollment = await _dbContext.StudentEnrollments
                                                        .Include(s => s.ClassSection)
                                                        .FirstOrDefaultAsync(s => s.Id == Guid.Parse(studentEnrollmentIdGuid), cancellationToken);

                if (studentEnrollment == null) return;

                var recurringFees = await _dbContext.FeeStructures
                    .Where(f => f.ClassId == studentEnrollment.ClassSection.ClassId && f.FeeType.IsRecurring)
                    .ToListAsync(cancellationToken);


                var firstMonth = DateTime.SpecifyKind(new DateTime(studentEnrollment.CreatedDate.Year, studentEnrollment.CreatedDate.Month, 1), DateTimeKind.Utc);

                var currentMonth = DateTime.SpecifyKind(new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1), DateTimeKind.Utc);

                foreach (var fee in recurringFees)
                {
                    var existingMonths = await _dbContext.StudentFees
                                                         .Where(sf => sf.StudentEnrollmentId == studentEnrollment.Id && sf.FeeStructureId == fee.Id && sf.FeeMonth.HasValue)
                                                         .Select(sf => sf.FeeMonth.Value.Month + sf.FeeMonth.Value.Year * 100)
                                                         .ToListAsync(cancellationToken);

                    for (var month = firstMonth; month <= currentMonth; month = month.AddMonths(1))
                    {
                        var monthUtc = DateTime.SpecifyKind(month, DateTimeKind.Utc);
                        var monthKey = monthUtc.Month + monthUtc.Year * 100;

                        if (!existingMonths.Contains(monthKey))
                        {
                            _dbContext.StudentFees.Add(new StudentFee
                            {
                                StudentEnrollmentId = studentEnrollment.Id,
                                FeeStructureId = fee.Id,
                                Amount = fee.Amount,
                                FeeMonth = monthUtc,
                                IsPaid = false,
                                CreatedDate = DateTime.UtcNow
                            });
                        }
                    }
                }

                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task PayStudentFeeAsync(Guid studentFeeId, decimal amount, string paymentMode, CancellationToken cancellationToken)
        {
            var studentFee = await _dbContext.StudentFees
                .Include(sf => sf.Payments)
                .FirstOrDefaultAsync(sf => sf.Id == studentFeeId, cancellationToken);

            if (studentFee == null) throw new Exception("Student fee not found");

            // Add payment
            studentFee.Payments.Add(new Payment
            {
                StudentFeeId = studentFee.Id,
                AmountPaid = amount,
                PaymentDate = DateTime.UtcNow,
                Method = paymentMode
            });

            // Update IsPaid if total paid >= fee amount
            if (studentFee.Payments.Sum(p => p.AmountPaid) >= studentFee.Amount)
                studentFee.IsPaid = true;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

    }
}
