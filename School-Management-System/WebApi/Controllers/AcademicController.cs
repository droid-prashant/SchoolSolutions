using Application.Academic.Dtos;
using Application.Academic.Interfaces;
using Application.Academic.ViewModels;
using Application.ClassSections.VieModels;
using Application.Common.Interfaces;
using Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcademicController : ApiBaseController
    {
        private readonly IAcademicService _academicService;
        public AcademicController(IAcademicService academicService)
        {
            _academicService = academicService;
        }
        [HttpGet]
        [Route("GetAllAcademicYear")]
        public async Task<List<AcademicViewModels>> GetAllAcademicYear(CancellationToken cancellationToken)
        {
            var result = await _academicService.GetAllAcademicYear(cancellationToken);
            return result;
        }

        [HttpPost]
        [Route("AddAcademicYear")]
        [HasPermission(PermissionNames.AcademicYearManage)]
        public async Task AddAcademicYear([FromBody] AcademicYearDto academicYearDto, CancellationToken cancellationToken)
        {
             await _academicService.AddAcademicYear(academicYearDto, cancellationToken);
        }

        [HttpPut]
        [Route("UpdateAcademicYear")]
        [HasPermission(PermissionNames.AcademicYearManage)]
        public async Task UpdateAcademicYear([FromBody] AcademicYearDto academicYearDto, [FromQuery] string academicYearId, CancellationToken cancellationToken)
        {
            await _academicService.UpdateAcademicYear(academicYearDto, academicYearId, cancellationToken);
        }
    }
}
