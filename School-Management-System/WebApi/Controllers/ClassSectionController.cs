using System.Threading;
using Application.ClassSections.Dtos;
using Application.ClassSections.Interfaces;
using Application.ClassSections.VieModels;
using Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassSectionController : ApiBaseController
    {
        private readonly IClassSectionService _classSectionService;
        public ClassSectionController(IClassSectionService classSectionService)
        {
            _classSectionService = classSectionService;
        }

        [HttpPost]
        [Route("AddClass")]
        [HasPermission(PermissionNames.ClassManage)]
        public async Task AddClass([FromBody] ClassRoomDto classRoomDto, CancellationToken cancellationToken)
        {
            await _classSectionService.AddClass(classRoomDto, cancellationToken);
        }

        [HttpPut]
        [Route("UpdateClass")]
        [HasPermission(PermissionNames.ClassManage)]
        public async Task UpdateClass([FromBody] ClassRoomDto classRoomDto, CancellationToken cancellationToken)
        {
            await _classSectionService.UpdateClass(classRoomDto, cancellationToken);
        }

        [HttpPost]
        [Route("AddSection")]
        [HasPermission(PermissionNames.ClassManage)]
        public async Task AddSection([FromBody] SectionDto section, CancellationToken cancellationToken)
        {
            await _classSectionService.AddSection(section, cancellationToken);
        }   
        
        [HttpPut]
        [Route("UpdateSection")]
        [HasPermission(PermissionNames.ClassManage)]
        public async Task UpdateSection([FromBody] SectionDto section, CancellationToken cancellationToken)
        {
            await _classSectionService.UpdateSection(section, cancellationToken);

        }

        [HttpPost]
        [Route("MapClassSection")]
        [HasPermission(PermissionNames.ClassManage)]
        public async Task CreateClassSection(ClassSectionDto classSectionDto, CancellationToken cancellationToken)
        {
            await _classSectionService.CreateClassSection(classSectionDto, cancellationToken);
        }

        [HttpGet]
        [Route("GetClassSections")]
        [HasPermission(PermissionNames.ClassManage, PermissionNames.StudentView)]
        public async Task<List<ClassSectionViewModel>> GetAllClassSections(CancellationToken cancellationToken)
        {
            var result = await _classSectionService.GetAllClassSections(cancellationToken);
            return result;
        }

        [HttpGet("GetClassDetails/{id}")]
        [HasPermission(PermissionNames.ClassManage, PermissionNames.StudentView)]
        public async Task<ClassSectionViewModel> GetClassDetails(Guid id, CancellationToken cancellationToken)
        {
            var result = await _classSectionService.GetClassSectionDetails(id, cancellationToken);
            return result;
        }

    }
}
