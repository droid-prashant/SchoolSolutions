using Application.Dashboard.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dashboard.Interfaces
{
    public interface IDashboardService
    {
        Task<int> GetStudentCount(CancellationToken cancellationToken);
        Task<int> GetCoursesCount(CancellationToken cancellationToken);
        Task<List<StudentsByClassViewModel>> GetStudentsByClass(CancellationToken cancellationToken);
    }
}
