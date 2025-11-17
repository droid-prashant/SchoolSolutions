using Application.Common.Interfaces;
using Application.Dashboard.Interfaces;
using Application.Dashboard.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Dashboard
{
    public class DasboardService : IDashboardService
    {
        private readonly IApplicationDbContext _context;
        public DasboardService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetCoursesCount(CancellationToken cancellationToken)
        {
            int coursesCount = await _context.Courses.CountAsync(cancellationToken);
            return coursesCount;
        }

        public async Task<int> GetStudentCount(CancellationToken cancellationToken)
        {
            int studentCount = await _context.StudentEnrollments.CountAsync(cancellationToken);
            return studentCount;
        }

        public async Task<List<StudentsByClassViewModel>> GetStudentsByClass(CancellationToken cancellationToken)
        {
            var result = await _context.StudentEnrollments
                                 .Include(x => x.ClassSection)
                                 .ThenInclude(x => x.ClassRoom)
                                 .Where(x => x.Student.isActive == true)
                                 .GroupBy(x => x.ClassSection.ClassRoom.Name)
                                 .Select(g => new StudentsByClassViewModel
                                 {
                                     ClassRoom = g.Key,
                                     StudentsCountBySections = g.Select(x => new StudentCountBySection
                                     {
                                         SectionName = x.ClassSection.Section.Name,
                                         StudentCount = x.ClassSection.StudentEnrollments.Count()
                                     }).ToList()
                                 }).ToListAsync(cancellationToken);

            return result;
        }
    }
}
