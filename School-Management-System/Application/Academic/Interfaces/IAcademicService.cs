using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Academic.Dtos;
using Application.Academic.ViewModels;

namespace Application.Academic.Interfaces
{
    public interface IAcademicService
    {
        Task AddAcademicYear(AcademicYearDto academicYearDto, CancellationToken cancellationToken);
        Task UpdateAcademicYear(AcademicYearDto academicYearDto, string academicYearId, CancellationToken cancellationToken);
        Task<List<AcademicViewModels>> GetAllAcademicYear(CancellationToken cancellationToken);
    }
}
