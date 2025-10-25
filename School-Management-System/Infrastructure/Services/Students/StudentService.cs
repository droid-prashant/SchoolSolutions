using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Students.Dtos;
using Application.Students.Interfaces;
using Application.Students.ViewModels;
using Azure.Core;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Students
{
    public class StudentService : IStudentService
    {
        private readonly IApplicationDbContext _context;
        public StudentService(IApplicationDbContext context)
        {
            _context = context;
        }

        private async Task MapStudentFeesAsync(Student student, CancellationToken cancellationToken)
        {
            var selectedClass = await _context.ClassSections.FirstOrDefaultAsync(x => x.Id == student.ClassSectionId);
            if (selectedClass == null)
            {
                return ;
            }
            var admissionFee = await _context.FeeTypes.FirstOrDefaultAsync(x => x.Name == "Admission Fee");
            if (admissionFee == null)
            {
                return;
            }
            var classFees = await _context.FeeStructures
                .Where(f => f.ClassId == selectedClass.ClassId && f.FeeType.Id == admissionFee.Id)
                .ToListAsync(cancellationToken);

            if (!classFees.Any())
                return;

            var studentFees = classFees.Select(fee => new StudentFee
            {
                StudentId = student.Id,
                FeeStructureId = fee.Id,
                Amount = fee.Amount,
                IsPaid = false,
                CreatedDate = DateTime.UtcNow
            }).ToList();

            await _context.StudentFees.AddRangeAsync(studentFees, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task AddStudentAsync(StudentDto addStudent, CancellationToken cancellationToken)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var student = new Student
                {
                    FirstName = addStudent.FirstName,
                    LastName = addStudent.LastName,
                    FatherName = addStudent.FatherName,
                    MotherName = addStudent.MotherName,
                    GrandFatherName = addStudent.GrandFatherName,
                    Gender = addStudent.Gender,
                    Age = addStudent.Age,
                    DateOfBirth = addStudent.Dob,
                    Municipality = addStudent.Municipality,
                    WardNo = addStudent.WardNo,
                    Address = addStudent.Address,
                    ContactNumber = addStudent.ContactNumber,
                    ClassSectionId = Guid.Parse(addStudent.ClassSectionId),
                    CreatedDate = DateTime.UtcNow
                };
                _context.Students.Add(student);
                await _context.SaveChangesAsync(cancellationToken);
                await MapStudentFeesAsync(student, cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }

        }

        public async Task<List<StudentViewModel>> GetStudentAsync(CancellationToken cancellationToken)
        {
            var students = await _context.Students.Include(x => x.ClassSection).ThenInclude(x => x.ClassRoom).Select(x => new StudentViewModel
            {
                Id = x.Id,
                Name = x.FirstName + " " + x.LastName,
                Address = x.Address,
                Age = x.Age,
                ClassRoom = x.ClassSection.ClassRoom.Name,
                Section = x.ClassSection.Section.Name,
                Gender = x.Gender == 1 ? "Male" : "Female",
                Municipality = x.Municipality,
                WardNo = x.WardNo,
                DateOfBirth = x.DateOfBirth.ToString("yyyy-MM-dd")

            })
                .ToListAsync(cancellationToken);
            return students;
        }
        public async Task<List<StudentViewModel>> GetStudentByClassIdAsync(Guid classRooomId, CancellationToken cancellationToken)
        {
            var students = await _context.Students.Include(x => x.ClassSection)
                                                   .ThenInclude(x => x.ClassRoom)
                                                  .Where(x => x.ClassSection.ClassId == classRooomId)
                                                  .Select(x => new StudentViewModel
                                                  {
                                                      Id = x.Id,
                                                      Name = x.FirstName + " " + x.LastName,
                                                      Address = x.Address,
                                                      Age = x.Age,
                                                      ClassRoom = x.ClassSection.ClassRoom.Name,
                                                      Section = x.ClassSection.Section.Name,
                                                      Gender = x.Gender == 1 ? "Male" : "Female"
                                                  })
                .ToListAsync(cancellationToken);
            return students;
        }

        public async Task<List<StudentViewModel>> GetStudentByClassSectionId(string classSectionId, CancellationToken cancellationToken)
        {
            var students = await _context.Students.Include(x => x.ClassSection)
                                                  .ThenInclude(x => x.ClassRoom)
                                                 .Where(x => x.ClassSection.Id == Guid.Parse(classSectionId))
                                                 .Select(x => new StudentViewModel
                                                 {
                                                     Id = x.Id,
                                                     Name = x.FirstName + " " + x.LastName,
                                                     Address = x.Address,
                                                     Age = x.Age,
                                                     ClassRoom = x.ClassSection.ClassRoom.Name,
                                                     Section = x.ClassSection.Section.Name,
                                                     Gender = x.Gender == 1 ? "Male" : "Female"
                                                 })
               .ToListAsync(cancellationToken);
            return students;
        }
    }
}
