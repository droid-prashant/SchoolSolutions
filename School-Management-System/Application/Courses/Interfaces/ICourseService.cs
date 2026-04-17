using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Courses.Dtos;
using Application.Courses.ViewModels;

namespace Application.Courses.Interfaces
{
    public interface ICourseService
    {
        Task AddCourse(CourseDto courseDto, CancellationToken cancellationToken);
        Task UpdateCourse(CourseDto courseDto, CancellationToken cancellationToken);
        Task AddClassCourse(ClassCourseDto classCourseDto, CancellationToken cancellationToken);
        Task UpdateClassCourse(ClassCourseDto classCourseDto, CancellationToken cancellationToken);
        Task DeleteClassCourse(string classCourseId, CancellationToken cancellationToken);
        Task<List<ClassCreditCourseViewModel>> GetClassCourseByClassId(Guid classRoomId, CancellationToken cancellationToken);
        Task<List<ClassCreditCourseViewModel>> GetAllClassCourse(CancellationToken cancellationToken);
    }
}
