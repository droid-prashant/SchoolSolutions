using Application.Common.Interfaces;
using Application.Students.Dtos;
using Application.Students.Interfaces;
using Application.Students.ViewModels;
using Azure.Core;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Services.Students
{
    public class StudentService : IStudentService
    {
        private readonly IApplicationDbContext _context;
        private readonly UserResolver _userResolver;
        public StudentService(IApplicationDbContext context, UserResolver userResolver)
        {
            _context = context;
            _userResolver = userResolver;
        }
        private async Task MapStudentFeesAsync(StudentEnrollment studentEnrollment, CancellationToken cancellationToken)
        {
            var selectedClassSection = await _context.ClassSections.FirstOrDefaultAsync(x => x.Id == studentEnrollment.ClassSectionId);
            if (selectedClassSection == null)
            {
                return;
            }
            studentEnrollment.ClassSection = selectedClassSection;
            var admissionFee = await _context.FeeTypes.FirstOrDefaultAsync(x => x.Name == "Admission Fee");
            if (admissionFee == null)
            {
                return;
            }
            var classFees = await _context.FeeStructures.Include(x => x.ClassRoom.ClassSections)
                .Where(f => f.ClassRoom.Id == studentEnrollment.ClassSection.ClassId && f.FeeType.Id == admissionFee.Id)
                .ToListAsync(cancellationToken);

            if (!classFees.Any())
                return;

            var studentFees = classFees.Select(fee => new StudentFee
            {
                StudentEnrollmentId = studentEnrollment.Id,
                FeeStructureId = fee.Id,
                Amount = fee.Amount,
                IsPaid = false,
                CreatedDate = DateTime.UtcNow
            }).ToList();

            await _context.StudentFees.AddRangeAsync(studentFees, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
        private async Task StudentEnrollent(Student student, string classSectionId, CancellationToken cancellationToken)
        {
            var studentEnrollment = new StudentEnrollment
            {
                StudentId = student.Id,
                AcademicYearId = Guid.Parse(_userResolver.AcademicYearId),
                ClassSectionId = Guid.Parse(classSectionId),
                RegistrationNumber = null,
                SymbolNumber = null,
                CreatedDate = DateTime.UtcNow,
                IsPromoted = true
            };
            await _context.StudentEnrollments.AddAsync(studentEnrollment);
            await _context.SaveChangesAsync(cancellationToken);
            await MapStudentFeesAsync(studentEnrollment, cancellationToken);
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
                    ProvinceId = addStudent.ProvinceId,
                    DistrictId = addStudent.DistrictId,
                    MunicipalityId = addStudent.MunicipalityId,
                    ParentContactNumber = addStudent.ParentContactNumber,
                    ParentEmail = addStudent.ParentEmail,
                    WardNo = addStudent.WardNo,
                    Address = addStudent.Address,
                    ContactNumber = addStudent.ContactNumber,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = Guid.Parse(_userResolver.UserId)
                };
                _context.Students.Add(student);
                await _context.SaveChangesAsync(cancellationToken);
                await StudentEnrollent(student, addStudent.ClassSectionId, cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }

        }
        public async Task UpdateStudentAsync(StudentDto studentDto, string studentId, CancellationToken cancellationToken)
        {
            var existingStudent = await _context.Students.FirstOrDefaultAsync(x => x.Id == Guid.Parse(studentId));
            if (existingStudent != null)
            {
                existingStudent.FirstName = studentDto.FirstName;
                existingStudent.LastName = studentDto.LastName;
                existingStudent.Address = studentDto.Address;
                existingStudent.FatherName = studentDto.FatherName;
                existingStudent.MotherName = studentDto.MotherName;
                existingStudent.GrandFatherName = studentDto.GrandFatherName;
                existingStudent.Age = studentDto.Age;
                existingStudent.Gender = studentDto.Gender;
                existingStudent.ParentEmail = studentDto.ParentEmail;
                existingStudent.ParentContactNumber = studentDto.ParentContactNumber;
                existingStudent.ProvinceId = studentDto.ProvinceId;
                existingStudent.DistrictId = studentDto.DistrictId;
                existingStudent.MunicipalityId = studentDto.MunicipalityId;
                existingStudent.WardNo = studentDto.WardNo;
                existingStudent.ContactNumber = studentDto.ContactNumber;
                existingStudent.DateOfBirth = studentDto.Dob;
                existingStudent.ModifiedBy = Guid.Parse(_userResolver.UserId);
                existingStudent.ModifiedDate = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task AssignRollNumbersAsync(string classSectionId, CancellationToken cancellationToken)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var classGuid = Guid.Parse(classSectionId);
                var yearGuid = Guid.Parse(_userResolver.AcademicYearId);

                var enrollments = await _context.StudentEnrollments
                    .Include(x => x.Student)
                    .Where(x => x.ClassSectionId == classGuid && x.AcademicYearId == yearGuid)
                    .OrderBy(x => x.Student.FirstName.ToLower())
                    .ThenBy(x => x.Student.LastName.ToLower())
                    .ToListAsync(cancellationToken);

                int rollNumber = 1;
                foreach (var enrollment in enrollments)
                {
                    enrollment.RollNumber = rollNumber++;
                }

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
        public async Task AssignRegistrationAndSymbolNumber(StudentEnrollmentDto studentEnrollmentDto, string studentEnrollmentId, CancellationToken cancellationToken)
        {
            var studentEnrollment = await _context.StudentEnrollments.FirstOrDefaultAsync(x => x.Id == Guid.Parse(studentEnrollmentId));
            if (studentEnrollment != null)
            {
                studentEnrollment.RegistrationNumber = studentEnrollmentDto.RegistrationNumber;
                studentEnrollment.SymbolNumber = studentEnrollmentDto.SymbolNumber;

                await _context.SaveChangesAsync(cancellationToken);
            }
        }
        public async Task<List<StudentViewModel>> GetStudentAsync(CancellationToken cancellationToken)
        {
            var students = await _context.StudentEnrollments.Include(x => x.ClassSection).ThenInclude(x => x.ClassRoom).Select(x => new StudentViewModel
            {
                Id = x.Id,
                Name = x.Student.FirstName + " " + x.Student.LastName,
                Address = x.Student.Province.ProvinceName + ", " + x.Student.District.DistrictName + ", " + x.Student.Municipality.MunicipalityName,
                Age = x.Student.Age,
                ClassRoom = x.ClassSection.ClassRoom.Name,
                Section = x.ClassSection.Section.Name,
                Gender = x.Student.Gender == 1 ? "Male" : "Female",
                ProvinceName = x.Student.Province.ProvinceName,
                ProvinceId = x.Student.ProvinceId.ToString(),
                DistrictName = x.Student.District.DistrictName,
                DistrictId = x.Student.DistrictId.ToString(),
                MunicipalityName = x.Student.Municipality.MunicipalityName,
                MunicipalityId = x.Student.MunicipalityId.ToString(),
                WardNo = x.Student.WardNo,
                RegistrationNumber = x.RegistrationNumber != null ? x.RegistrationNumber : "",
                SymbolNumber = x.SymbolNumber != null ? x.SymbolNumber : x.RollNumber.ToString(),
                DateOfBirth = x.Student.DateOfBirth.ToString("dd-MM-yyyy")

            }).OrderBy(x => x.Name)
              .ToListAsync(cancellationToken);
            return students;
        }
        public async Task<List<StudentEnrollmentViewModel>> GetRegAndSymCompliantEnrolledStudents(string classSectionId, CancellationToken cancellationToken)
        {
            var result = await _context.StudentEnrollments.Include(x => x.ClassSection)
                                                    .Include(x => x.Student)
                                                   .Where(x => x.ClassSectionId == Guid.Parse(classSectionId) &&
                                                            (x.ClassSection.ClassRoom.OrderNumber == 8 || x.ClassSection.ClassRoom.OrderNumber == 10))
                                                   .Select(x => new StudentEnrollmentViewModel
                                                   {
                                                       Id = x.Id.ToString(),
                                                       Name = x.Student.FirstName + ' ' + x.Student.LastName,
                                                       Class = x.ClassSection.ClassRoom.Name,
                                                       ClassSectionId = x.ClassSectionId.ToString(),
                                                       RegistrationNumber = x.RegistrationNumber != null ? x.RegistrationNumber : "-",
                                                       SymbolNumber = x.SymbolNumber != null ? x.SymbolNumber : "-"
                                                   }).ToListAsync(cancellationToken);

            return result;


        }
        public async Task<List<StudentViewModel>> GetStudentByClassIdAsync(Guid classRooomId, CancellationToken cancellationToken)
        {
            var students = await _context.StudentEnrollments.Include(x => x.ClassSection)
                                                   .ThenInclude(x => x.ClassRoom)
                                                  .Where(x => x.ClassSection.ClassId == classRooomId)
                                                  .Select(x => new StudentViewModel
                                                  {
                                                      Id = x.Id,
                                                      Name = x.Student.FirstName + " " + x.Student.LastName,
                                                      Address = x.Student.Address,
                                                      Age = x.Student.Age,
                                                      ClassRoom = x.ClassSection.ClassRoom.Name,
                                                      Section = x.ClassSection.Section.Name,
                                                      Gender = x.Student.Gender == 1 ? "Male" : "Female",
                                                      WardNo = x.Student.WardNo,
                                                      DateOfBirth = x.Student.DateOfBirth.ToString("dd-MM-yyyy"),
                                                  }).OrderBy(x => x.Name)
                                                    .ToListAsync(cancellationToken);
            return students;
        }
        public async Task<List<StudentViewModel>> GetStudentByClassSectionId(string classSectionId, CancellationToken cancellationToken)
        {
            var students = await _context.StudentEnrollments.Include(x => x.ClassSection)
                                                  .ThenInclude(x => x.ClassRoom)
                                                 .Where(x => x.ClassSection.Id == Guid.Parse(classSectionId))
                                                 .Select(x => new StudentViewModel
                                                 {
                                                     Id = x.Id,
                                                     Name = x.Student.FirstName + " " + x.Student.LastName,
                                                     Address = x.Student.Address,
                                                     Age = x.Student.Age,
                                                     ClassRoom = x.ClassSection.ClassRoom.Name,
                                                     Section = x.ClassSection.Section.Name,
                                                     Gender = x.Student.Gender == 1 ? "Male" : "Female"
                                                 }).OrderBy(x => x.Name)
                                                   .ToListAsync(cancellationToken);
            return students;
        }
    }
}
