using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Fees.Dtos;
using Application.Fees.ViewModel;

namespace Application.Fees.Interfaces
{
    public interface IFeeService
    {
        Task<List<FeeTypeViewModel>> GetFeeType(CancellationToken cancellationToken);
        Task<List<FeeStructureViewModel>> GetFeeStructure(string classId, CancellationToken cancellationToken);
        Task<StudentFeeSummaryViewModel> GetStudentFeeSummary(string studentEnrollmentId, string classSectionId, CancellationToken cancellationToken);
        Task AddFeeType(FeeTypeDto feeTypeDto, CancellationToken cancellationToken);
        Task AddFeeStructure(FeeStructureDto feeStructureDto, CancellationToken cancellationToken);
        Task EnsureMissingMonthlyFeesAsync(string studentEnrollmentIdGuid, CancellationToken cancellationToken);
        Task UpdateFeeType(FeeTypeDto feeTypeDto, string feeTypeId, CancellationToken cancellationToken);
        Task UpdateFeeSctucture(FeeStructureDto feeStructureDto, string feeStructureId, CancellationToken cancellationToken);
        Task PayStudentFeeAsync(Guid studentFeeId, decimal amount, string paymentMode, CancellationToken cancellationToken);
    }
}
