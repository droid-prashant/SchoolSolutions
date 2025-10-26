using System.Threading;
using Application.ClassSections.Dtos;
using Application.ClassSections.Interfaces;
using Application.ClassSections.VieModels;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
        [Route("GetClassRooms")]
        public async Task<List<ClassRoomViewModel>> GetAllClassRooms(CancellationToken cancellationToken)
        {
            var result = await _classSectionService.GetAllClassRooms(cancellationToken);
            return result;
        }

        [HttpGet]
        [Route("GetSections")]
        public async Task<List<SectionViewModel>> GetAllSections(CancellationToken cancellationToken)
        {
            var result = await _classSectionService.GetAllSections(cancellationToken);
            return result;
        }

        [HttpPost]
        [Route("AddClass")]
        public async Task AddClass([FromBody] ClassRoomDto classRoomDto, CancellationToken cancellationToken)
        {
            await _classSectionService.AddClass(classRoomDto, cancellationToken);
        }

        [HttpPut]
        [Route("UpdateClass")]
        public async Task UpdateClass([FromBody] ClassRoomDto classRoomDto, CancellationToken cancellationToken)
        {
            await _classSectionService.UpdateClass(classRoomDto, cancellationToken);
        }

        [HttpPost]
        [Route("AddSection")]
        public async Task AddSection([FromBody] SectionDto section, CancellationToken cancellationToken)
        {
            await _classSectionService.AddSection(section, cancellationToken);
        }   
        
        [HttpPut]
        [Route("UpdateSection")]
        public async Task UpdateSection([FromBody] SectionDto section, CancellationToken cancellationToken)
        {
            await _classSectionService.UpdateSection(section, cancellationToken);

        }

        [HttpPost]
        [Route("MapClassSection")]
        public async Task CreateClassSection(ClassSectionDto classSectionDto, CancellationToken cancellationToken)
        {
            await _classSectionService.CreateClassSection(classSectionDto, cancellationToken);
        }

        [HttpGet]
        [Route("GetClassSections")]
        public async Task<List<ClassSectionViewModel>> GetAllClassSections(CancellationToken cancellationToken)
        {
            var result = await _classSectionService.GetAllClassSections(cancellationToken);
            return result;
        }

        [HttpGet("GetClassDetails/{id}")]
        public async Task<ClassSectionViewModel> GetClassDetails(Guid id, CancellationToken cancellationToken)
        {
            var result = await _classSectionService.GetClassSectionDetails(id, cancellationToken);
            return result;
        }

    }
}
