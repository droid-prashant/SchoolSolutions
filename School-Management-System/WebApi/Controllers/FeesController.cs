using Application.Fees.Dtos;
using Application.Fees.Interfaces;
using Application.Fees.ViewModel;
using Domain;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<List<FeeTypeViewModel>> GetFeeType(CancellationToken cancellationToken)
        {
            var result = await _feesService.GetFeeType(cancellationToken);
            return result;
        }

        [HttpGet]
        [Route("GetFeeStructure")]
        public async Task<List<FeeStructureViewModel>> GetFeeStructure([FromQuery] string classId, CancellationToken cancellationToken)
        {
            var result = await _feesService.GetFeeStructure(classId, cancellationToken);
            return result;
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost]
        [Route("AddFeeType")]
        public Task AddFeeType([FromBody] FeeTypeDto feeTypeDto, CancellationToken cancellationToken)
        {
            return _feesService.AddFeeType(feeTypeDto, cancellationToken);
        }

        [HttpPost]
        [Route("AddFeeStructure")]
        public Task AddFeeStructure([FromBody] FeeStructureDto feeStructureDto, CancellationToken cancellationToken)
        {
            return _feesService.AddFeeStructure(feeStructureDto, cancellationToken);
        }

        [HttpPut]
        [Route("UpdateFeeType")]
        public Task UpdateFeeType([FromBody] FeeTypeDto feeTypeDto, [FromQuery] string feeTypeId, CancellationToken cancellationToken)
        {
            return _feesService.UpdateFeeType(feeTypeDto, feeTypeId, cancellationToken);
        }

        [HttpPut]
        [Route("UpdateFeeStructure")]
        public Task UpdateFeeStructure([FromBody] FeeStructureDto feeStructureDto, [FromQuery] string feeStructureId, CancellationToken cancellationToken)
        {
            return _feesService.UpdateFeeSctucture(feeStructureDto, feeStructureId, cancellationToken);
        }

        [HttpPost]
        [Route("PayStudentFee")]
        public Task PayStudentFeeAsync([FromQuery] Guid studentFeeId, decimal amount, string paymentMode, CancellationToken cancellationToken)
        {
            return _feesService.PayStudentFeeAsync(studentFeeId, amount, paymentMode, cancellationToken);
        }

        [HttpGet]
        [Route("GetStudentFeeSummary")]
        public async Task<StudentFeeSummaryViewModel> GetStudentFeeSummary([FromQuery] string studentId, [FromQuery] string classSectionId, CancellationToken cancellationToken)
        {
            var result = await _feesService.GetStudentFeeSummary(studentId, classSectionId, cancellationToken);
            return result;
        }

        [HttpGet]
        [Route("EnsureMissingMonthlyFees")]
        public async Task EnsureMissingMonthlyFeesAsync([FromQuery] string studentId, CancellationToken cancellationToken)
        {
            await _feesService.EnsureMissingMonthlyFeesAsync(studentId, cancellationToken);
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
