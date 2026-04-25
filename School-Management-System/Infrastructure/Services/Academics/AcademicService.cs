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
                    StartDateNp = academicYearDto.StartDateNp,
                    EndDateNp = academicYearDto.EndDateNp,
                    StartDateEn = academicYearDto.StartDateEn,
                    EndDateEn = academicYearDto.EndDateEn,
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
                StartDateNp = x.StartDateNp,
                EndDateNp = x.EndDateNp,
                StartDateEn = x.StartDateEn,
                EndDateEn = x.EndDateEn, 
                IsActive = x.IsActive,
            }).ToListAsync(cancellationToken);

            return result;
        }

        public async Task UpdateAcademicYear(AcademicYearDto academicYearDto, string academicYearId, CancellationToken cancellationToken)
        {
            var existingAcademicYearDetail = _context.AcademicYears.FirstOrDefault(x => x.Id == Guid.Parse(academicYearId));
            if (existingAcademicYearDetail != null)
            {
                existingAcademicYearDetail.YearName = academicYearDto.YearName;
                existingAcademicYearDetail.StartDateNp = academicYearDto.StartDateNp;
                existingAcademicYearDetail.EndDateNp = academicYearDto.EndDateNp;
                existingAcademicYearDetail.StartDateEn = academicYearDto.StartDateEn;
                existingAcademicYearDetail.EndDateEn = academicYearDto.EndDateEn;
                existingAcademicYearDetail.IsActive = academicYearDto.IsActive;

                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
