using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.ClassSections.Dtos;
using Application.ClassSections.VieModels;

namespace Application.ClassSections.Interfaces
{
    public interface IClassSectionService
    {
        Task<List<ClassRoomViewModel>> GetAllClassRooms(CancellationToken cancellationToken);
        Task<List<SectionViewModel>> GetAllSections(CancellationToken cancellationToken);
        Task CreateClassSection(ClassSectionDto classSectionDto, CancellationToken cancellationToken);
        Task AddSection(SectionDto section, CancellationToken cancellationToken);
        Task AddClass(ClassRoomDto classRoomDto, CancellationToken cancellationToken);
        Task<List<ClassSectionViewModel>> GetAllClassSections(CancellationToken cancellationToken);
        Task<ClassSectionViewModel> GetClassSectionDetails(Guid classSectionId, CancellationToken cancellationToken);
    }
}
