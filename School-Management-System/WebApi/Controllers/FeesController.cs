using Application.Fees.Dtos;
using Application.Fees.Interfaces;
using Application.Fees.ViewModel;
using Domain.Constants;
using Domain;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeesController : ControllerBase
    {
        private readonly IFeeService _feesService;
        public FeesController(IFeeService feeService)
        {
            _feesService = feeService;
        }

        [HttpGet]
        [Route("GetFeeType")]
        [HasPermission(PermissionNames.FeeView, PermissionNames.FeeCreate)]
        public async Task<List<FeeTypeViewModel>> GetFeeType(CancellationToken cancellationToken)
        {
            var result = await _feesService.GetFeeType(cancellationToken);
            return result;
        }

        [HttpGet]
        [Route("GetFeeStructure")]
        [HasPermission(PermissionNames.FeeView, PermissionNames.FeeCreate)]
        public async Task<List<FeeStructureViewModel>> GetFeeStructure([FromQuery] string classId, CancellationToken cancellationToken)
        {
            var result = await _feesService.GetFeeStructure(classId, cancellationToken);
            return result;
        }

        [HttpGet]
        [Route("GetFeeReport")]
        [HasPermission(PermissionNames.FeeView)]
        public async Task<FeeReportViewModel?> GetFeeReport([FromQuery] string classSectionId, CancellationToken cancellationToken)
        {
            return await _feesService.GetFeeReport(classSectionId, cancellationToken);
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost]
        [Route("AddFeeType")]
        [HasPermission(PermissionNames.FeeCreate)]
        public Task AddFeeType([FromBody] FeeTypeDto     feeTypeDto, CancellationToken cancellationToken)
        {
            return _feesService.AddFeeType(feeTypeDto, cancellationToken);
        }

        [HttpPost]
        [Route("AddFeeStructure")]
        [HasPermission(PermissionNames.FeeCreate)]
        public Task AddFeeStructure([FromBody] FeeStructureDto feeStructureDto, CancellationToken cancellationToken)
        {
            return _feesService.AddFeeStructure(feeStructureDto, cancellationToken);
        }

        [HttpPost]
        [Route("ApplyFeeAdjustment")]
        [HasPermission(PermissionNames.FeeCreate)]
        public Task ApplyFeeAdjustment([FromBody] FeeAdjustmentDto feeAdjustmentDto, CancellationToken cancellationToken)
        {
            return _feesService.ApplyFeeAdjustmentAsync(feeAdjustmentDto, cancellationToken);
        }

        [HttpPut]
        [Route("UpdateFeeType")]
        [HasPermission(PermissionNames.FeeCreate)]
        public Task UpdateFeeType([FromBody] FeeTypeDto feeTypeDto, [FromQuery] string feeTypeId, CancellationToken cancellationToken)
        {
            return _feesService.UpdateFeeType(feeTypeDto, feeTypeId, cancellationToken);
        }

        [HttpPut]
        [Route("UpdateFeeStructure")]
        [HasPermission(PermissionNames.FeeCreate)]
        public Task UpdateFeeStructure([FromBody] FeeStructureDto feeStructureDto, [FromQuery] string feeStructureId, CancellationToken cancellationToken)
        {
            return _feesService.UpdateFeeSctucture(feeStructureDto, feeStructureId, cancellationToken);
        }

        [HttpPost]
        [Route("PayStudentFee")]
        [HasPermission(PermissionNames.FeeCreate)]
        public Task PayStudentFeeAsync([FromQuery] Guid studentFeeId, [FromQuery] Guid currentStudentEnrollmentId, [FromQuery] decimal amount, [FromQuery] string paymentMode, CancellationToken cancellationToken)
        {
            return _feesService.PayStudentFeeAsync(studentFeeId, currentStudentEnrollmentId, amount, paymentMode, cancellationToken);
        }

        [HttpGet]
        [Route("GetStudentFeeSummary")]
        [HasPermission(PermissionNames.FeeView, PermissionNames.FeeCreate)]
        public async Task<StudentFeeSummaryViewModel> GetStudentFeeSummary([FromQuery] string studentEnrollmentIdGuid, [FromQuery] string classSectionId, CancellationToken cancellationToken)
        {
            var result = await _feesService.GetStudentFeeSummary(studentEnrollmentIdGuid, classSectionId, cancellationToken);
            return result;
        }

        [HttpGet]
        [Route("EnsureMissingMonthlyFees")]
        [HasPermission(PermissionNames.FeeCreate)]
        public async Task EnsureMissingMonthlyFeesAsync([FromQuery] string studentEnrollmentIdGuid, CancellationToken cancellationToken)
        {
            await _feesService.EnsureMissingMonthlyFeesAsync(studentEnrollmentIdGuid, cancellationToken);
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
