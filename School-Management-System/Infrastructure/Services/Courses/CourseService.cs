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
            var course = new Course
            {
                Name = courseDto.Name,
            };
            _context.Courses.Add(course);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<CourseViewModel>> GetCourse(CancellationToken cancellationToken)
        {
            var courseList = await _context.Courses.Select(x => new CourseViewModel
            {
                Id = x.Id,
                Name = x.Name,
            }).ToListAsync(cancellationToken);

            return courseList;
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
                TheoryCreditHour = x.TheoryCreditHour,
                PracticalCreditHour = x.TheoryCreditHour,
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
                                                                  TheoryCreditHour = x.TheoryCreditHour,
                                                                  PracticalCreditHour = x.TheoryCreditHour,
                                                                  TheoryFullMarks = x.TheoryFullMarks,
                                                                  PracticalFullMarks = x.PracticalFullMarks,
                                                              }).ToListAsync(cancellationToken);

            return courseList;
        }

        public async Task AddClassCourse(ClassCourseDto classCourseDto, CancellationToken cancellationToken)
        {
            var classCourse = new ClassCourse
            {
                ClassRoomId = Guid.Parse(classCourseDto.ClassRoomId),
                CourseId = Guid.Parse(classCourseDto.CourseId),
                PracticalCreditHour = classCourseDto.TheoryCreditHour,
                TheoryCreditHour = classCourseDto.TheoryCreditHour,
                TheoryFullMarks = classCourseDto.TheoryFullMarks,
                PracticalFullMarks = classCourseDto.PracticalFullMarks,
                CreatedDate = DateTime.UtcNow
            };

            await _context.ClassCourses.AddAsync(classCourse);
            await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task UpdateClassCourse(ClassCourseDto classCourseDto, CancellationToken cancellationToken)
        {
            var existingData = new ClassCourse();
            if (classCourseDto.ClassCourseId != null)
            {
                existingData = _context.ClassCourses.Where(x => x.Id == Guid.Parse(classCourseDto.ClassCourseId)).FirstOrDefault();
            }


            existingData.ClassRoomId = Guid.Parse(classCourseDto.ClassRoomId);
            existingData.CourseId = Guid.Parse(classCourseDto.CourseId);
            existingData.PracticalCreditHour = classCourseDto.TheoryCreditHour;
            existingData.TheoryCreditHour = classCourseDto.TheoryCreditHour;
            existingData.TheoryFullMarks = classCourseDto.TheoryFullMarks;
            existingData.PracticalFullMarks = classCourseDto.PracticalFullMarks;
            existingData.CreatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
