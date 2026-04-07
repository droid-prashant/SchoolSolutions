using Application.Common.Interfaces;
using Application.Students.Dtos;
using Application.Students.Interfaces;
using Application.Students.ViewModels;
using Azure.Core;
using Domain;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
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
        private bool CheckCharacterCertificateTaken(Guid studentId)
        {
            var isCertificateTaken = _context.studentCharacterCertificateLogs.Any(x => x.StudentId == studentId);
            return isCertificateTaken;
        }
        private bool CheckTransferCertificateTaken(Guid studentId)
        {
            var isCertificateTaken = _context.studentTransferCertificateLogs.Any(x => x.StudentId == studentId);
            return isCertificateTaken;
        }
        private async Task SetStudentInActive(Guid studentId, CancellationToken cancellationToken)
        {
            var student = await _context.Students.FirstOrDefaultAsync(x => x.Id == studentId);
            if (student != null)
            {
                student.isActive = false;
                await _context.SaveChangesAsync(cancellationToken);
            }
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
        private async Task StudentEnrollent(Student student, Guid classSectionId, int rollNumber, CancellationToken cancellationToken)
        {
            var studentEnrollment = new StudentEnrollment
            {
                StudentId = student.Id,
                AcademicYearId = Guid.Parse(_userResolver.AcademicYearId),
                ClassSectionId = classSectionId,
                RegistrationNumber = null,
                SymbolNumber = null,
                RollNumber = rollNumber,
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
                var classSectionId = await _context.ClassSections.Where(x => x.ClassId == Guid.Parse(addStudent.ClassRoomId) && x.SectionId == Guid.Parse(addStudent.SectionId)).Select(x => x.Id).FirstOrDefaultAsync();
                var student = new Student
                {
                    FirstName = addStudent.FirstName,
                    LastName = addStudent.LastName,
                    FatherName = addStudent.FatherName,
                    MotherName = addStudent.MotherName,
                    GrandFatherName = addStudent.GrandFatherName,
                    Gender = addStudent.Gender,
                    Age = addStudent.Age,
                    DateOfBirthNp = addStudent.DobNp,
                    DateOfBirthEn = addStudent.DobEn,
                    ProvinceId = addStudent.ProvinceId,
                    DistrictId = addStudent.DistrictId,
                    MunicipalityId = addStudent.MunicipalityId,
                    ParentContactNumber = addStudent.ParentContactNumber,
                    ParentEmail = addStudent.ParentEmail,
                    WardNo = addStudent.WardNo,
                    ContactNumber = addStudent.ContactNumber,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = Guid.Parse(_userResolver.UserId),
                    isActive = true
                };
                _context.Students.Add(student);
                await _context.SaveChangesAsync(cancellationToken);
                await StudentEnrollent(student, classSectionId, addStudent.RollNumber, cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new Exception(ex.Message);
            }

        }
        public async Task UpdateStudentAsync(StudentDto studentDto, CancellationToken cancellationToken)
        {
            if (studentDto.Id == null)
            {
                throw new Exception("Invalid Request");
            }
            var existingStudent = await _context.Students.FirstOrDefaultAsync(x => x.Id == Guid.Parse(studentDto.Id));
            if (existingStudent == null)
            {
                throw new Exception("No Data to update");
            }
            else
            {
                existingStudent.FirstName = studentDto.FirstName;
                existingStudent.LastName = studentDto.LastName;
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
                existingStudent.DateOfBirthNp = studentDto.DobNp;
                existingStudent.DateOfBirthEn = studentDto.DobEn;
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
            var students = await _context.StudentEnrollments.Include(x => x.Student).Include(x => x.ClassSection)
                                                            .ThenInclude(x => x.ClassRoom)
                                                            .Where(x => x.Student.isActive == true)
                                                            .Select(x => new StudentViewModel
                                                            {
                                                                Id = x.StudentId,
                                                                StudentEnrollmentId = x.Id,
                                                                FirstName = x.Student.FirstName,
                                                                LastName = x.Student.LastName,
                                                                Address = x.Student.Province.ProvinceName + ", " + x.Student.District.DistrictName + ", " + x.Student.Municipality.MunicipalityName + " - " + x.Student.WardNo,
                                                                Age = x.Student.Age,
                                                                GrandFatherName = x.Student.GrandFatherName,
                                                                FatherName = x.Student.FatherName,
                                                                MotherName = x.Student.MotherName,
                                                                ClassRoomId = x.ClassSection.ClassRoom.Id.ToString(),
                                                                SectionId = x.ClassSection.Section.Id.ToString(),
                                                                ContactNumber = x.Student.ContactNumber,
                                                                ParentContactNumber = x.Student.ParentContactNumber,
                                                                ParentEmail = x.Student.ParentEmail,
                                                                Gender = x.Student.Gender,
                                                                ProvinceName = x.Student.Province.ProvinceName,
                                                                ProvinceId = x.Student.ProvinceId,
                                                                DistrictName = x.Student.District.DistrictName,
                                                                DistrictId = x.Student.DistrictId,
                                                                MunicipalityName = x.Student.Municipality.MunicipalityName,
                                                                MunicipalityId = x.Student.MunicipalityId,
                                                                WardNo = x.Student.WardNo,
                                                                RegistrationNumber = x.RegistrationNumber != null ? x.RegistrationNumber : "",
                                                                SymbolNumber = x.SymbolNumber != null ? x.SymbolNumber : x.RollNumber.ToString(),
                                                                DateOfBirthNp = x.Student.DateOfBirthNp,
                                                                DateOfBirthEn = x.Student.DateOfBirthEn,
                                                                ClassRoomName = x.ClassSection.ClassRoom.Name,
                                                                SectionName = x.ClassSection.Section.Name,
                                                                RollNumber = (int)(x.RollNumber)
                                                            }).OrderBy(x => x.FirstName).ThenBy(x => x.LastName)
              .ToListAsync(cancellationToken);
            return students;
        }
        public async Task<List<StudentCertificateViewModel>> GetStudentCertificateDataAsync(string classSectionId, CancellationToken cancellationToken)
        {
            try
            {
                var enrolledStudent = await _context.StudentEnrollments.Include(x => x.ClassSection).FirstOrDefaultAsync(x => x.ClassSection.Id == Guid.Parse(classSectionId));
                var studentFirstEnrollment = await _context.StudentEnrollments.Include(x => x.ClassSection).ThenInclude(x => x.ClassRoom).OrderBy(x => x.CreatedDate).FirstOrDefaultAsync(x => x.StudentId == enrolledStudent.StudentId);
                var students = await _context.StudentEnrollments.Include(x => x.ClassSection)
                                                                .ThenInclude(x => x.ClassRoom)
                                                                .Where(x => x.Student.isActive == true && x.ClassSection.Id == Guid.Parse(classSectionId)).Select(x => new StudentCertificateViewModel
                                                                {
                                                                    Id = x.Id,
                                                                    StudentId = x.StudentId,
                                                                    Name = x.Student.FirstName + " " + x.Student.LastName,
                                                                    Address = x.Student.Province.ProvinceName + ", " + x.Student.District.DistrictName + ", " + x.Student.Municipality.MunicipalityName + " - " + x.Student.WardNo,
                                                                    Age = x.Student.Age,
                                                                    ClassRoom = x.ClassSection.ClassRoom.Name,
                                                                    FirstAdmittedClass = studentFirstEnrollment.ClassSection.ClassRoom.Name,
                                                                    Section = x.ClassSection.Section.Name,
                                                                    WardNo = x.Student.WardNo,
                                                                    RegistrationNumber = x.RegistrationNumber != null ? x.RegistrationNumber : "",
                                                                    SymbolNumber = x.SymbolNumber != null ? x.SymbolNumber : x.RollNumber.ToString(),
                                                                    DateOfBirth = x.Student.DateOfBirthNp,
                                                                    AcademicYear = x.AcademicYear.YearName,
                                                                    AdmittedDate = x.Student.CreatedDate,
                                                                    IssueDate = DateTime.UtcNow,
                                                                    FatherName = x.Student.FatherName,
                                                                    MotherName = x.Student.MotherName,
                                                                    Gender = x.Student.Gender,
                                                                    GPA = x.ExamResults.OrderByDescending(x => x.CreatedDate)
                                                                            .Select(x => x.GPA)
                                                                            .FirstOrDefault(),
                                                                    ExamType = x.ExamResults.OrderByDescending(x => x.CreatedDate)
                                                                                 .Select(x => x.ExamType)
                                                                                 .First(),
                                                                    ExamHeld = x.ExamResults.OrderByDescending(x => x.CreatedDate)
                                                                                 .Select(x => x.CreatedDate)
                                                                                 .First(),
                                                                }).OrderBy(x => x.Name)
                  .ToListAsync(cancellationToken);

                foreach (var student in students)
                {
                    student.IsCharacterCertificateTaken = CheckCharacterCertificateTaken(student.StudentId);
                    student.IsTransferCertificateTaken = CheckTransferCertificateTaken(student.StudentId);
                }
                return students;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<StudentEnrollmentViewModel>> GetRegAndSymCompliantEnrolledStudents(string classSectionId, CancellationToken cancellationToken)
        {
            var result = await _context.StudentEnrollments.Include(x => x.ClassSection)
                                                          .Include(x => x.Student)
                                                          .Where(x => x.Student.isActive == true &&
                                                                      x.ClassSectionId == Guid.Parse(classSectionId) &&
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
                                                      FirstName = x.Student.FirstName,
                                                      LastName = x.Student.LastName,
                                                      Address = x.Student.Province.ProvinceName + ", " + x.Student.District.DistrictName + ", " + x.Student.Municipality.MunicipalityName + " - " + x.Student.WardNo,
                                                      Age = x.Student.Age,
                                                      ClassRoomId = x.ClassSection.ClassRoom.Id.ToString(),
                                                      SectionId = x.ClassSection.Section.Id.ToString(),
                                                      Gender = x.Student.Gender,
                                                      WardNo = x.Student.WardNo,
                                                      DateOfBirthNp = x.Student.DateOfBirthNp,
                                                      DateOfBirthEn = x.Student.DateOfBirthEn
                                                  }).OrderBy(x => x.FirstName).ThenBy(x => x.LastName)
                                                    .ToListAsync(cancellationToken);
            return students;
        }
        public async Task<List<StudentViewModel>> GetStudentByClassSectionId(string classSectionId, CancellationToken cancellationToken)
        {
            var students = await _context.StudentEnrollments.Include(x => x.ClassSection)
                                                            .ThenInclude(x => x.ClassRoom)
                                                            .Where(x => x.Student.isActive == true &&
                                                                   x.ClassSection.Id == Guid.Parse(classSectionId))
                                                            .Select(x => new StudentViewModel
                                                            {
                                                                Id = x.Id,
                                                                FirstName = x.Student.FirstName,
                                                                LastName = x.Student.LastName,
                                                                DateOfBirthNp = x.Student.DateOfBirthNp,
                                                                DateOfBirthEn = x.Student.DateOfBirthEn,
                                                                ClassRoomId = x.ClassSection.ClassRoom.Id.ToString(),
                                                                SectionId = x.ClassSection.Section.Id.ToString(),
                                                                Gender = x.Student.Gender
                                                            }).OrderBy(x => x.FirstName).ThenBy(x => x.LastName)
                                                   .ToListAsync(cancellationToken);
            return students;
        }
        public async Task AddStudentCertificateLog(StudentCertificateDto studentCertificateDto, CancellationToken cancellationToken)
        {
            var enrolledStudent = await _context.StudentEnrollments.Include(x => x.Student).FirstOrDefaultAsync(x => x.Id == Guid.Parse(studentCertificateDto.StudentEnrollmentId));
            bool isCharacterCertificateLogged = await _context.studentCharacterCertificateLogs.AnyAsync(x => x.StudentId == enrolledStudent.StudentId);
            bool isTransferCertificateLogged = await _context.studentTransferCertificateLogs.AnyAsync(x => x.StudentId == enrolledStudent.StudentId);
            if (studentCertificateDto.certificateType == CertificateType.CharacterCertificate)
            {
                var characterCertificate = new StudentCharacterCertificateLog
                {
                    StudentId = enrolledStudent.StudentId,
                    CreatedBy = Guid.Parse(_userResolver.UserId),
                    CreatedDate = DateTime.UtcNow,
                    CertificateNumber = studentCertificateDto.CertificateNumber
                };
                await _context.studentCharacterCertificateLogs.AddAsync(characterCertificate);
                if (isTransferCertificateLogged)
                {
                    await SetStudentInActive(enrolledStudent.StudentId, cancellationToken);
                }
                await _context.SaveChangesAsync(cancellationToken);
            }
            else if (studentCertificateDto.certificateType == CertificateType.TransferCertificate)
            {
                var transferCertificate = new StudentTransferCertificateLog
                {
                    StudentId = enrolledStudent.StudentId,
                    CreatedBy = Guid.Parse(_userResolver.UserId),
                    CreatedDate = DateTime.UtcNow,
                    CertificateNumber = studentCertificateDto.CertificateNumber
                };
                await _context.studentTransferCertificateLogs.AddAsync(transferCertificate);
                if (isCharacterCertificateLogged)
                {
                    await SetStudentInActive(enrolledStudent.StudentId, cancellationToken);
                }
                await _context.SaveChangesAsync(cancellationToken);
            }
            else
            {
                throw new Exception("Invalid Certificate Type");
            }
        }
        public async Task<StudentCertificateLogViewModel> GetStudentCertificateLog(CertificateType certificateType, CancellationToken cancellationToken)
        {
            if (certificateType == CertificateType.CharacterCertificate)
            {
                var result = await _context.studentCharacterCertificateLogs.OrderByDescending(x => x.CreatedDate)
                                                                           .Select(x => new StudentCertificateLogViewModel
                                                                           {
                                                                               Id = x.Id.ToString(),
                                                                               StudentId = x.StudentId.ToString(),
                                                                               CertificateNumber = x.CertificateNumber
                                                                           }).FirstOrDefaultAsync();
                if (result != null)
                {
                    return result;
                }
                else
                {
                    return new StudentCertificateLogViewModel { Id = "", StudentId = "", CertificateNumber = 0 };
                }
            }
            else if (certificateType == CertificateType.TransferCertificate)
            {
                var result = await _context.studentTransferCertificateLogs.OrderByDescending(x => x.CreatedDate)
                                                                          .Select(x => new StudentCertificateLogViewModel
                                                                          {
                                                                              Id = x.Id.ToString(),
                                                                              StudentId = x.StudentId.ToString(),
                                                                              CertificateNumber = x.CertificateNumber
                                                                          }).FirstOrDefaultAsync();

                if (result != null)
                {
                    return result;
                }
                else
                {
                    return new StudentCertificateLogViewModel { Id = "", StudentId = "", CertificateNumber = 0 };
                }
            }
            else
            {
                throw new Exception("Invalid Certificate Type");
            }
        }
    }
}
