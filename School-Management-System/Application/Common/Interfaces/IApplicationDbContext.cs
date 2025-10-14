using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Teacher> Teachers { get; }
        DbSet<Student> Students{ get; }
        DbSet<Course> Courses { get; }
        DbSet<ClassRoom> ClassRooms{ get; }
        DbSet<Section> Sections{ get; }
        DbSet<ClassSection> ClassSections{ get; }
        DbSet<ClassCourse> ClassCourses { get; }
        DbSet<SubjectMark> SubjectMarks{ get; }
        DbSet<ExamResult> ExamResults{ get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
