using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Courses.Dtos;
using Application.Courses.Interfaces;
using Application.Courses.ViewModels;
using Azure.Core;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Courses
{
    public class CourseService : ICourseService
    {
        private readonly IApplicationDbContext _context;
        public CourseService(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddCourse(CourseDto courseDto, CancellationToken cancellationToken)
        {
            bool checkExistingCourse = await _context.Courses.AnyAsync(x => x.Name == courseDto.Name);
            if (!checkExistingCourse)
            {
                var course = new Course
                {
                    Name = courseDto.Name,
                };
                _context.Courses.Add(course);
                await _context.SaveChangesAsync(cancellationToken);
            }
            else
            {
                throw new Exception("Course already exist");
            }
        }
        public async Task UpdateCourse(CourseDto courseDto, CancellationToken cancellationToken)
        {
            var existingCourse = await _context.Courses.FirstOrDefaultAsync(x => x.Id == Guid.Parse(courseDto.Id));
            if (existingCourse != null)
            {
                existingCourse.Name = courseDto.Name;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
        public async Task<List<ClassCreditCourseViewModel>> GetAllClassCourse(CancellationToken cancellationToken)
        {
            var courseList = await _context.ClassCourses.Select(x => new ClassCreditCourseViewModel
            {
                CourseId = x.CourseId,
                CourseName = x.Course.Name,
                ClassRoomId = x.ClassRoomId,
                ClassName = x.ClassRoom.Name,
                ClassCreditCourseId = x.Id,
                IsTheoryRequired = x.IsTheoryRequired,
                IsPracticalRequired = x.IsPracticalRequired,
                TheoryCreditHour = x.TheoryCreditHour,
                PracticalCreditHour = x.PracticalCreditHour,
                TheoryFullMarks = x.TheoryFullMarks,
                PracticalFullMarks = x.PracticalFullMarks,
            }).ToListAsync(cancellationToken);

            return courseList;
        }
        public async Task<List<ClassCreditCourseViewModel>> GetClassCourseByClassId(Guid classRoomId, CancellationToken cancellationToken)
        {
            var courseList = await _context.ClassCourses.Where(x => x.ClassRoomId == classRoomId)
                                                              .Select(x => new ClassCreditCourseViewModel
                                                              {
                                                                  CourseId = x.CourseId,
                                                                  CourseName = x.Course.Name,
                                                                  ClassRoomId = x.ClassRoomId,
                                                                  ClassName = x.ClassRoom.Name,
                                                                  ClassCreditCourseId = x.Id,
                                                                  IsTheoryRequired = x.IsTheoryRequired,
                                                                  IsPracticalRequired = x.IsPracticalRequired,
                                                                  TheoryCreditHour = x.TheoryCreditHour,
                                                                  PracticalCreditHour = x.PracticalCreditHour,
                                                                  TheoryFullMarks = x.TheoryFullMarks,
                                                                  PracticalFullMarks = x.PracticalFullMarks,
                                                              }).ToListAsync(cancellationToken);

            return courseList;
        }
        public async Task AddClassCourse(ClassCourseDto classCourseDto, CancellationToken cancellationToken)
        {
            var existingClassCourse = await _context.ClassCourses.FirstOrDefaultAsync(x => x.CourseId == Guid.Parse(classCourseDto.CourseId) && x.ClassRoomId == Guid.Parse(classCourseDto.ClassRoomId));
            if(existingClassCourse != null)
            {
                throw new Exception("Course already mapped");
            }
            var classCourse = new ClassCourse
            {
                ClassRoomId = Guid.Parse(classCourseDto.ClassRoomId),
                CourseId = Guid.Parse(classCourseDto.CourseId),
                IsTheoryRequired = classCourseDto.IsTheoryRequired,
                IsPracticalRequired = classCourseDto.IsPracticalRequired,
                TheoryCreditHour = classCourseDto.IsTheoryRequired ? classCourseDto.TheoryCreditHour : null,
                PracticalCreditHour = classCourseDto.IsPracticalRequired ? classCourseDto.PracticalCreditHour : null,
                TheoryFullMarks = classCourseDto.IsTheoryRequired ? classCourseDto.TheoryFullMarks : null,
                PracticalFullMarks = classCourseDto.IsPracticalRequired ? classCourseDto.PracticalFullMarks : null,
                CreatedDate = DateTime.UtcNow
            };

            await _context.ClassCourses.AddAsync(classCourse);
            await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task UpdateClassCourse(ClassCourseDto classCourseDto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(classCourseDto.ClassCourseId))
            {
                throw new ArgumentException("ClassCourseId is required.");
            }

            var existingData = await _context.ClassCourses
                .FirstOrDefaultAsync(x => x.Id == Guid.Parse(classCourseDto.ClassCourseId), cancellationToken);

            if (existingData == null)
            {
                throw new KeyNotFoundException("Class course not found.");
            }

            existingData.ClassRoomId = Guid.Parse(classCourseDto.ClassRoomId);
            existingData.CourseId = Guid.Parse(classCourseDto.CourseId);
            existingData.IsTheoryRequired = classCourseDto.IsTheoryRequired;
            existingData.IsPracticalRequired = classCourseDto.IsPracticalRequired;
            existingData.TheoryCreditHour = classCourseDto.IsTheoryRequired ? classCourseDto.TheoryCreditHour : null;
            existingData.PracticalCreditHour = classCourseDto.IsPracticalRequired ? classCourseDto.PracticalCreditHour : null;
            existingData.TheoryFullMarks = classCourseDto.IsTheoryRequired ? classCourseDto.TheoryFullMarks : null;
            existingData.PracticalFullMarks = classCourseDto.IsPracticalRequired ? classCourseDto.PracticalFullMarks : null;
            existingData.CreatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteClassCourse(string classCourseId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(classCourseId))
            {
                throw new ArgumentException("ClassCourseId is required.");
            }

            if (!Guid.TryParse(classCourseId, out var parsedId))
            {
                throw new ArgumentException("Invalid ClassCourseId.");
            }

            var existingData = await _context.ClassCourses
                .FirstOrDefaultAsync(x => x.Id == parsedId, cancellationToken);

            if (existingData == null)
            {
                throw new KeyNotFoundException("Class course not found.");
            }

            _context.ClassCourses.Remove(existingData);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
