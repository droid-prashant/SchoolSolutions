using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Academic.Dtos;
using Application.Academic.Interfaces;
using Application.Academic.ViewModels;
using Application.Common.Interfaces;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Academics
{
    public class AcademicService : IAcademicService
    {
        private readonly IApplicationDbContext _context;
        public AcademicService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAcademicYear(AcademicYearDto academicYearDto, CancellationToken cancellationToken)
        {
            bool checkExitingAcademicYear = await _context.AcademicYears.AnyAsync(x => x.YearName == academicYearDto.YearName);
            if (!checkExitingAcademicYear)
            {
                var academicYear = new AcademicYear
                {
                    YearName = academicYearDto.YearName,
                    StartDate = academicYearDto.StartDate,
                    EndDate = academicYearDto.EndDate,
                    IsActive = academicYearDto.IsActive,
                    CreatedDate = DateTime.UtcNow
                };
                await _context.AcademicYears.AddAsync(academicYear);
                await _context.SaveChangesAsync(cancellationToken);
            }
            else
            {
                throw new Exception("Academic Year already Exist");
            }
        }

        public async Task<List<AcademicViewModels>> GetAllAcademicYear(CancellationToken cancellationToken)
        {
            var result = await _context.AcademicYears.Select(x => new AcademicViewModels
            {
                Id = x.Id.ToString(),
                YearName = x.YearName,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                StartDateFormatted = x.StartDate.ToString("yyyy-MM-dd"),
                EndDateFormatted = x.EndDate.ToString("yyyy-MM-dd")
            }).ToListAsync(cancellationToken);

            return result;
        }

        public async Task UpdateAcademicYear(AcademicYearDto academicYearDto, string academicYearId, CancellationToken cancellationToken)
        {
            var existingAcademicYearDetail = _context.AcademicYears.FirstOrDefault(x => x.Id == Guid.Parse(academicYearId));
            if (existingAcademicYearDetail != null)
            {
                existingAcademicYearDetail.YearName = academicYearDto.YearName;
                existingAcademicYearDetail.StartDate = academicYearDto.StartDate;
                existingAcademicYearDetail.EndDate = academicYearDto.EndDate;
                existingAcademicYearDetail.IsActive = academicYearDto.IsActive;

                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
