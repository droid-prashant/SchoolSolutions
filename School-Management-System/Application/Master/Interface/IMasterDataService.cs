using Application.Academic.ViewModels;
using Application.ClassSections.VieModels;
using Application.Courses.ViewModels;
using Application.Master.ViewModels;

namespace Application.Master.Interface
{
    public interface IMasterDataService
    {
        Task<List<ProvinceViewModel>> GetAllProvince(CancellationToken cancellationToken);
        Task<List<ClassRoomViewModel>> GetAllClassRooms(CancellationToken cancellationToken);
        Task<List<SectionViewModel>> GetAllSections(CancellationToken cancellationToken);
        Task<List<CourseViewModel>> GetAllCourse(CancellationToken cancellationToken);
        Task<List<AcademicViewModels>> GetAllAcademicYears(CancellationToken cancellationToken);
    }
}
