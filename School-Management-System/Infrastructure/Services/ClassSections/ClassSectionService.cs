using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
                                                                Section = x.Section.Name,
                                                                OrderNumber = x.ClassRoom.OrderNumber
                                                            }).OrderBy(x => x.OrderNumber)
                                                              .FirstOrDefaultAsync(cancellationToken);
            return result;
        }

        public async Task AddSection(SectionDto section, CancellationToken cancellationToken)
        {
            bool checkSectionExist = await _context.Sections.AnyAsync(x => x.Name == section.Name);
            if (!checkSectionExist)
            {
                var sectionObj = new Section
                {
                    Name = section.Name,
                };
                await _context.Sections.AddAsync(sectionObj);
                await _context.SaveChangesAsync(cancellationToken);
            }
            else
            {
                throw new Exception("Section Already Exist");
            }
        }

        public async Task AddClass(ClassRoomDto classRoomDto, CancellationToken cancellationToken)
        {
            bool checkClassExist = await _context.ClassRooms.AnyAsync(x => x.Name == classRoomDto.Name);
            if (!checkClassExist)
            {
                var classRoom = new ClassRoom
                {
                    Name = classRoomDto.Name,
                    AcademicYear = DateTime.UtcNow.Year.ToString(),
                    OrderNumber = classRoomDto.OrderNumber
                };
                await _context.ClassRooms.AddAsync(classRoom);
                await _context.SaveChangesAsync(cancellationToken);
            }
            else
            {
                throw new Exception("Class name already exist");
            }
        }

        public async Task UpdateClass(ClassRoomDto classRoomDto, CancellationToken cancellationToken)
        {
            var existingClass = await _context.ClassRooms.FirstOrDefaultAsync(x => x.Id == Guid.Parse(classRoomDto.Id));
            if (existingClass != null)
            {
                existingClass.Name = classRoomDto.Name;
                existingClass.OrderNumber = classRoomDto.OrderNumber;

                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task UpdateSection(SectionDto section, CancellationToken cancellationToken)
        {
            var existingSection = await _context.Sections.FirstOrDefaultAsync(x => x.Id == Guid.Parse(section.SectionId));
            if (existingSection != null)
            {
                existingSection.Name = section.Name;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
