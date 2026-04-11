using Application.ClassSections.VieModels;
using Application.Courses.ViewModels;
using Application.Master.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Master.Interface
{
    public interface IMasterDataService
    {
        Task<List<ProvinceViewModel>> GetAllProvince(CancellationToken cancellationToken);
        Task<List<ClassRoomViewModel>> GetAllClassRooms(CancellationToken cancellationToken);
        Task<List<SectionViewModel>> GetAllSections(CancellationToken cancellationToken);
        Task<List<CourseViewModel>> GetAllCourse(CancellationToken cancellationToken);
    }
}
