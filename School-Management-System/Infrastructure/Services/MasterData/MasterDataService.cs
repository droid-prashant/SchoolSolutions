using Application.Academic.ViewModels;
using Application.ClassSections.VieModels;
using Application.Common.Interfaces;
using Application.Courses.ViewModels;
using Application.Master.Interface;
using Application.Master.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.MasterData
{
    public class MasterDataService : IMasterDataService
    {
        private readonly IApplicationDbContext _dbContext;

        public MasterDataService(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ProvinceViewModel>> GetAllProvince(CancellationToken cancellationToken)
        {
            var result = await _dbContext.Provinces.Select(x => new ProvinceViewModel
            {
                Id = x.Id,
                Name = x.ProvinceName,
                Districts = x.Districts.Select(y => new DistrictViewModel
                {
                    Id = y.Id,
                    Name = y.DistrictName,
                    Municipalities = y.Municipalities.Select(m => new MunicipalityViewModel
                    {
                        Id = m.Id,
                        Name = m.MunicipalityName
                    }).ToList()
                }).ToList()
            }).ToListAsync(cancellationToken);

            return result;
        }

        public async Task<List<ClassRoomViewModel>> GetAllClassRooms(CancellationToken cancellationToken)
        {
            var result = await _dbContext.ClassRooms.Include(x => x.ClassSections).Select(x => new ClassRoomViewModel
            {
                Id = x.Id,
                Name = x.Name,
                OrderNumber = x.OrderNumber,
                AcademicYear = x.AcademicYear,
                CreatedOn = x.CreatedDate,
                Sections = x.ClassSections.Where(c => c.ClassId == x.Id).Select(x => new SectionViewModel
                {
                    SectionId = x.SectionId.ToString(),
                    Name = x.Section.Name,
                    ClassSectionId = x.Id.ToString()
                }).ToList()
            }).OrderBy(x => x.OrderNumber)
              .ToListAsync(cancellationToken);

            return result;
        }

        public async Task<List<SectionViewModel>> GetAllSections(CancellationToken cancellationToken)
        {
            var result = await _dbContext.Sections.Select(x => new SectionViewModel
            {
                SectionId = x.Id.ToString(),
                Name = x.Name
            }).OrderBy(x => x.Name)
              .ToListAsync(cancellationToken);

            return result;
        }

        public async Task<List<CourseViewModel>> GetAllCourse(CancellationToken cancellationToken)
        {
            var courseList = await _dbContext.Courses.Select(x => new CourseViewModel
            {
                Id = x.Id,
                Name = x.Name
            }).ToListAsync(cancellationToken);

            return courseList;
        }

        public async Task<List<AcademicViewModels>> GetAllAcademicYears(CancellationToken cancellationToken)
        {
            var academicYears = await _dbContext.AcademicYears.Select(x => new
            {
                x.Id,
                x.YearName,
                x.IsActive,
                x.StartDateNp,
                x.EndDateNp,
                x.StartDateEn,
                x.EndDateEn
            }).OrderByDescending(x => x.StartDateEn)
              .ToListAsync(cancellationToken);

            var result = academicYears.Select(x => new AcademicViewModels
            {
                Id = x.Id.ToString(),
                YearName = x.YearName,
                IsActive = x.IsActive,
                StartDateNp = x.StartDateNp,
                EndDateNp = x.EndDateNp,
                StartDateEn = x.StartDateEn,
                EndDateEn = x.EndDateEn,
                StartDateFormatted = x.StartDateEn,
                EndDateFormatted = x.EndDateEn
            }).ToList();

            return result;
        }
    }
}
