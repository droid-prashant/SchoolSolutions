using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Application.ClassSections.Dtos;
using Application.ClassSections.Interfaces;
using Application.ClassSections.VieModels;
using Application.Common.Interfaces;
using Application.Students.ViewModels;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.ClassSections
{
    public class ClassSectionService : IClassSectionService
    {
        private readonly IApplicationDbContext _context;
        public ClassSectionService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateClassSection(ClassSectionDto classSectionDto, CancellationToken cancellationToken)
        {
            bool sectionExists = await _context.ClassSections.AnyAsync(x => x.ClassId == classSectionDto.ClassRoomId && (classSectionDto.SectionIdList.Contains(x.SectionId)));
            if (sectionExists)
            {
                var sectionsToDelete = await _context.ClassSections.Where(x => x.ClassId == classSectionDto.ClassRoomId).ToListAsync();

                _context.ClassSections.RemoveRange(sectionsToDelete);
            }
            foreach (var sectionId in classSectionDto.SectionIdList)
            {
                var classSection = new ClassSection
                {
                    ClassId = classSectionDto.ClassRoomId,
                    SectionId = sectionId
                };
                _context.ClassSections.Add(classSection);
            }
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<ClassRoomViewModel>> GetAllClassRooms(CancellationToken cancellationToken)
        {
            var result = await _context.ClassRooms.Include(x => x.ClassSections).Select(x => new ClassRoomViewModel
            {
                Id = x.Id,
                Name = x.Name,
                RoomNumber = x.RoomNumber,
                AcademicYear = x.AcademicYear,
                Sections = x.ClassSections.Where(c => c.ClassId == x.Id).Select(x => new SectionViewModel
                {
                    SectionId = x.SectionId.ToString(),
                    Name = x.Section.Name,
                    ClassSectionId = x.Id.ToString()
                }).ToList()
            }).OrderBy(x => x.Id).ToListAsync();
            return result;
        }


        public async Task<List<SectionViewModel>> GetAllSections(CancellationToken cancellationToken)
        {
            var result = await _context.Sections.Select(x => new SectionViewModel
            {
                SectionId = x.Id.ToString(),
                Name = x.Name

            }).ToListAsync();
            return result;
        }

        public async Task<List<ClassSectionViewModel>> GetAllClassSections(CancellationToken cancellationToken)
        {
            var result = await _context.ClassSections.Include(x => x.ClassRoom)
                                                     .Include(x => x.Section)
                                       .Select(x => new ClassSectionViewModel
                                       {
                                           Id = x.Id,
                                           ClassRoomName = x.ClassRoom.Name,
                                           Section = x.Section.Name
                                       }).ToListAsync();
            return result;
        }

        public async Task<ClassSectionViewModel> GetClassSectionDetails(Guid classSectionId, CancellationToken cancellationToken)
        {
            var result = await _context.ClassSections.Include(x => x.ClassRoom)
                                                            .Include(x => x.Section)
                                                            .Where(x => x.Id == classSectionId)
                                                            .Select(x => new ClassSectionViewModel
                                                            {
                                                                Id = x.Id,
                                                                ClassRoomName = x.ClassRoom.Name,
                                                                Section = x.Section.Name
                                                            }).FirstOrDefaultAsync();
            return null;
        }

        public Task AddSection(SectionDto section, CancellationToken cancellationToken)
        {
            var sectionObj = new Section
            {
                Name = section.Name,
            };
            _context.Sections.Add(sectionObj);
            _context.SaveChangesAsync(cancellationToken);
            return Task.CompletedTask;
        }

        public async Task AddClass(ClassRoomDto classRoomDto, CancellationToken cancellationToken)
        {
            var classRoom = new ClassRoom
            {
                Name = classRoomDto.Name,
                AcademicYear = DateTime.UtcNow.Year.ToString(),
                RoomNumber = classRoomDto.RoomNumber
            };
            await _context.ClassRooms.AddAsync(classRoom);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
