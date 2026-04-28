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
using Infrastructure.Services;

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

        private Guid GetCurrentAcademicYearId()
        {
            return _userResolver.GetAcademicYearGuidOrThrow();
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
                student.IsActive = false;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
        private async Task MapStudentFeesAsync(StudentEnrollment studentEnrollment, CancellationToken cancellationToken)
        {
            await MapStudentFeesAsync(studentEnrollment, GetCurrentAcademicYearId(), cancellationToken);
        }

        private async Task MapStudentFeesAsync(StudentEnrollment studentEnrollment, Guid academicYearId, CancellationToken cancellationToken)
        {
            var selectedClassSection = await _context.ClassSections.FirstOrDefaultAsync(x => x.Id == studentEnrollment.ClassSectionId);
            if (selectedClassSection == null)
            {
                return;
            }

            studentEnrollment.ClassSection = selectedClassSection;
            var classFees = await _context.FeeStructures
                .Include(x => x.FeeType)
                .Where(f => f.ClassId == studentEnrollment.ClassSection.ClassId && f.AcademicYearId == academicYearId)
                .ToListAsync(cancellationToken);

            if (!classFees.Any())
            {
                return;
            }

            var startMonth = new DateTime(studentEnrollment.CreatedDate.Year, studentEnrollment.CreatedDate.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var currentMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var studentFees = new List<StudentFee>();

            foreach (var fee in classFees)
            {
                if (fee.FeeType.IsRecurring)
                {
                    for (var month = startMonth; month <= currentMonth; month = month.AddMonths(1))
                    {
                        studentFees.Add(new StudentFee
                        {
                            StudentEnrollmentId = studentEnrollment.Id,
                            FeeStructureId = fee.Id,
                            Amount = fee.Amount,
                            FeeMonth = month,
                            IsPaid = false,
                            CreatedDate = DateTime.UtcNow
                        });
                    }
                }
                else
                {
                    studentFees.Add(new StudentFee
                    {
                        StudentEnrollmentId = studentEnrollment.Id,
                        FeeStructureId = fee.Id,
                        Amount = fee.Amount,
                        FeeMonth = null,
                        IsPaid = false,
                        CreatedDate = DateTime.UtcNow
                    });
                }
            }

            if (studentFees.Any())
            {
                await _context.StudentFees.AddRangeAsync(studentFees, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        private async Task<StudentEnrollment> CreateStudentEnrollmentAsync(Guid studentId, Guid classSectionId, Guid academicYearId, bool isPromoted, CancellationToken cancellationToken)
        {
            var studentEnrollment = new StudentEnrollment
            {
                StudentId = studentId,
                AcademicYearId = academicYearId,
                ClassSectionId = classSectionId,
                RegistrationNumber = null,
                SymbolNumber = null,
                RollNumber = null,
                CreatedDate = DateTime.UtcNow,
                EnrollmentDate = DateTime.UtcNow,
                IsPromoted = isPromoted
            };
            await _context.StudentEnrollments.AddAsync(studentEnrollment);
            await _context.SaveChangesAsync(cancellationToken);
            return studentEnrollment;
        }

        private async Task StudentEnrollent(Student student, Guid classSectionId, CancellationToken cancellationToken)
        {
            var currentAcademicYearId = GetCurrentAcademicYearId();
            var studentEnrollment = await CreateStudentEnrollmentAsync(student.Id, classSectionId, currentAcademicYearId, false, cancellationToken);
            await MapStudentFeesAsync(studentEnrollment, currentAcademicYearId, cancellationToken);
            await AssignRollNumbersInternalAsync(classSectionId, currentAcademicYearId, cancellationToken);
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
                    IsActive = true
                };
                _context.Students.Add(student);
                await _context.SaveChangesAsync(cancellationToken);
                await StudentEnrollent(student, classSectionId, cancellationToken);
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
                var currentAcademicYearId = GetCurrentAcademicYearId();
                var existingStudentEnrollment = await _context.StudentEnrollments.FirstOrDefaultAsync(x => x.Student.Id == existingStudent.Id && x.AcademicYearId == currentAcademicYearId);
                if (existingStudentEnrollment == null)
                {
                    throw new Exception("Student is not Enrolled");
                }
                else
                {
                    var previousClassSectionId = existingStudentEnrollment.ClassSectionId;
                    existingStudentEnrollment.ClassSectionId = Guid.Parse(studentDto.ClassSectionId);
                    existingStudentEnrollment.RollNumber = null;

                    await _context.SaveChangesAsync(cancellationToken);
                    await AssignRollNumbersAsync(existingStudentEnrollment.ClassSectionId.ToString(), cancellationToken);

                    if (previousClassSectionId != existingStudentEnrollment.ClassSectionId)
                    {
                        await AssignRollNumbersAsync(previousClassSectionId.ToString(), cancellationToken);
                    }

                    return;
                }

            }
            await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task AssignRollNumbersAsync(string classSectionId, CancellationToken cancellationToken)
        {
            await AssignRollNumbersInternalAsync(Guid.Parse(classSectionId), GetCurrentAcademicYearId(), cancellationToken);
        }

        private async Task AssignRollNumbersInternalAsync(Guid classSectionId, Guid academicYearId, CancellationToken cancellationToken)
        {
            var hasActiveTransaction = _context.Database.CurrentTransaction != null;
            await using var transaction = hasActiveTransaction
                ? null
                : await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var enrollments = await _context.StudentEnrollments
                    .Include(x => x.Student)
                    .Where(x => x.ClassSectionId == classSectionId && x.AcademicYearId == academicYearId)
                    .OrderBy(x => x.Student.FirstName.ToLower())
                    .ThenBy(x => x.Student.LastName.ToLower())
                    .ToListAsync(cancellationToken);

                int rollNumber = 1;
                foreach (var enrollment in enrollments)
                {
                    enrollment.RollNumber = rollNumber++;
                }

                await _context.SaveChangesAsync(cancellationToken);
                if (transaction != null)
                {
                    await transaction.CommitAsync(cancellationToken);
                }
            }
            catch (Exception)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                }
                throw;
            }
        }
        public async Task AssignRegistrationAndSymbolNumber(StudentEnrollmentDto studentEnrollmentDto, string studentEnrollmentId, CancellationToken cancellationToken)
        {
            var currentAcademicYearId = GetCurrentAcademicYearId();
            var studentEnrollment = await _context.StudentEnrollments.FirstOrDefaultAsync(x => x.Id == Guid.Parse(studentEnrollmentId) && x.AcademicYearId == currentAcademicYearId);
            if (studentEnrollment != null)
            {
                studentEnrollment.RegistrationNumber = studentEnrollmentDto.RegistrationNumber;
                studentEnrollment.SymbolNumber = studentEnrollmentDto.SymbolNumber;

                await _context.SaveChangesAsync(cancellationToken);
            }
        }
        public async Task<List<StudentViewModel>> GetStudentAsync(CancellationToken cancellationToken)
        {
            var currentAcademicYearId = GetCurrentAcademicYearId();
            var students = await _context.StudentEnrollments.Include(x => x.Student).Include(x => x.ClassSection)
                                                            .ThenInclude(x => x.ClassRoom)
                                                            .Where(x => x.Student.IsActive == true && x.AcademicYearId == currentAcademicYearId)
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
                                                                ClassSectionId = x.ClassSection.Id.ToString(),
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
                var currentAcademicYearId = GetCurrentAcademicYearId();
                var classSectionGuid = Guid.Parse(classSectionId);
                var enrolledStudent = await _context.StudentEnrollments
                    .Include(x => x.ClassSection)
                    .FirstOrDefaultAsync(x => x.ClassSection.Id == classSectionGuid && x.AcademicYearId == currentAcademicYearId, cancellationToken);

                if (enrolledStudent == null)
                {
                    return new List<StudentCertificateViewModel>();
                }

                var studentFirstEnrollment = await _context.StudentEnrollments.Include(x => x.ClassSection).ThenInclude(x => x.ClassRoom).OrderBy(x => x.CreatedDate).FirstOrDefaultAsync(x => x.StudentId == enrolledStudent.StudentId);
                var students = await _context.StudentEnrollments.Include(x => x.ClassSection)
                                                                .ThenInclude(x => x.ClassRoom)
                                                                .Where(x => x.Student.IsActive == true &&
                                                                            x.ClassSection.Id == classSectionGuid &&
                                                                            x.AcademicYearId == currentAcademicYearId)
                                                                .Select(x => new StudentCertificateViewModel
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
            var currentAcademicYearId = GetCurrentAcademicYearId();
            var result = await _context.StudentEnrollments.Include(x => x.ClassSection)
                                                          .Include(x => x.Student)
                                                          .Where(x => x.Student.IsActive == true &&
                                                                      x.AcademicYearId == currentAcademicYearId &&
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
        public async Task<List<PromotionCandidateViewModel>> GetPromotionCandidates(string classSectionId, int examType, CancellationToken cancellationToken)
        {
            var currentAcademicYearId = GetCurrentAcademicYearId();
            var classSectionGuid = Guid.Parse(classSectionId);

            var currentClassSection = await _context.ClassSections
                .Include(x => x.ClassRoom)
                .Include(x => x.Section)
                .FirstOrDefaultAsync(x => x.Id == classSectionGuid, cancellationToken);

            if (currentClassSection == null)
            {
                return new List<PromotionCandidateViewModel>();
            }

            var nextClass = await _context.ClassRooms
                .Where(x => x.OrderNumber == currentClassSection.ClassRoom.OrderNumber + 1)
                .FirstOrDefaultAsync(cancellationToken);

            var nextClassSection = nextClass == null
                ? null
                : await _context.ClassSections
                    .Include(x => x.ClassRoom)
                    .Include(x => x.Section)
                    .FirstOrDefaultAsync(x => x.ClassId == nextClass.Id && x.SectionId == currentClassSection.SectionId, cancellationToken);

            var enrollments = await _context.StudentEnrollments
                .Include(x => x.Student)
                    .ThenInclude(x => x.StudentEnrollments)
                        .ThenInclude(x => x.ClassSection)
                            .ThenInclude(x => x.ClassRoom)
                .Include(x => x.ClassSection)
                    .ThenInclude(x => x.ClassRoom)
                .Include(x => x.ClassSection)
                    .ThenInclude(x => x.Section)
                .Include(x => x.ExamResults.Where(er => er.ExamType == examType))
                    .ThenInclude(er => er.SubjectMarks)
                .Where(x => x.AcademicYearId == currentAcademicYearId && x.ClassSectionId == classSectionGuid && x.Student.IsActive)
                .OrderBy(x => x.RollNumber ?? int.MaxValue)
                .ThenBy(x => x.Student.FirstName)
                .ThenBy(x => x.Student.LastName)
                .ToListAsync(cancellationToken);

            var result = new List<PromotionCandidateViewModel>();

            foreach (var enrollment in enrollments)
            {
                var latestResult = enrollment.ExamResults
                    .OrderByDescending(x => x.CreatedDate)
                    .FirstOrDefault();

                var evaluation = EvaluatePromotionStatus(latestResult);
                var targetClassName = nextClassSection?.ClassRoom?.Name ?? nextClass?.Name ?? string.Empty;
                var targetSectionName = nextClassSection?.Section?.Name ?? string.Empty;
                var hasTarget = nextClassSection != null;
                var hasHigherClassEnrollment = enrollment.Student.StudentEnrollments.Any(x =>
                    x.Id != enrollment.Id &&
                    x.ClassSection?.ClassRoom != null &&
                    x.ClassSection.ClassRoom.OrderNumber > enrollment.ClassSection.ClassRoom.OrderNumber &&
                    x.EnrollmentDate >= enrollment.EnrollmentDate);
                var isAlreadyPromoted = enrollment.IsPromoted || hasHigherClassEnrollment;
                var isPromotable = evaluation.IsPassed && hasTarget && !isAlreadyPromoted;
                var resultStatus = isAlreadyPromoted ? "Promoted" : evaluation.Status;

                result.Add(new PromotionCandidateViewModel
                {
                    StudentEnrollmentId = enrollment.Id,
                    StudentName = $"{enrollment.Student.FirstName} {enrollment.Student.LastName}".Trim(),
                    RollNumber = enrollment.RollNumber,
                    CurrentClassName = enrollment.ClassSection.ClassRoom.Name,
                    CurrentSectionName = enrollment.ClassSection.Section.Name,
                    ExamType = examType,
                    GPA = latestResult?.GPA,
                    ResultStatus = resultStatus,
                    IsPromotable = isPromotable,
                    IsAlreadyPromoted = isAlreadyPromoted,
                    TargetClassName = targetClassName,
                    TargetSectionName = targetSectionName,
                    Remarks = GetPromotionRemarks(evaluation.Status, hasTarget, nextClass, isAlreadyPromoted)
                });
            }

            return result;
        }

        public async Task<PromotionExecutionResultViewModel> PromoteStudentsAsync(PromoteStudentsDto request, CancellationToken cancellationToken)
        {
            var currentAcademicYearId = GetCurrentAcademicYearId();
            var targetAcademicYear = await ValidateAndGetTargetAcademicYearAsync(request.TargetAcademicYearId, currentAcademicYearId, cancellationToken);
            var classSectionGuid = Guid.Parse(request.ClassSectionId);
            var currentClassSection = await GetRequiredClassSectionAsync(classSectionGuid, cancellationToken);
            var targetClassSection = await ResolveDefaultPromotionTargetAsync(currentClassSection, cancellationToken);

            return await ExecuteStudentTransitionAsync(
                request,
                currentAcademicYearId,
                targetAcademicYear,
                classSectionGuid,
                targetClassSection,
                enrollment => EvaluatePromotionStatus(enrollment.ExamResults.OrderByDescending(x => x.CreatedDate).FirstOrDefault()).IsPassed,
                markSourceAsPromoted: true,
                missingTargetMessage: "No target class section configured for normal promotion.",
                cancellationToken);
        }

        public async Task<PromotionExecutionResultViewModel> SustainStudentsAsync(PromoteStudentsDto request, CancellationToken cancellationToken)
        {
            var currentAcademicYearId = GetCurrentAcademicYearId();
            var targetAcademicYear = await ValidateAndGetTargetAcademicYearAsync(request.TargetAcademicYearId, currentAcademicYearId, cancellationToken);
            var classSectionGuid = Guid.Parse(request.ClassSectionId);
            var currentClassSection = await GetRequiredClassSectionAsync(classSectionGuid, cancellationToken);

            return await ExecuteStudentTransitionAsync(
                request,
                currentAcademicYearId,
                targetAcademicYear,
                classSectionGuid,
                currentClassSection,
                enrollment => !EvaluatePromotionStatus(enrollment.ExamResults.OrderByDescending(x => x.CreatedDate).FirstOrDefault()).IsPassed,
                markSourceAsPromoted: false,
                missingTargetMessage: string.Empty,
                cancellationToken);
        }

        public async Task<PromotionExecutionResultViewModel> ManuallyPromoteStudentsAsync(PromoteStudentsDto request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.TargetClassSectionId))
            {
                throw new Exception("Target class section is required for manual promotion.");
            }

            var currentAcademicYearId = GetCurrentAcademicYearId();
            var targetAcademicYear = await ValidateAndGetTargetAcademicYearAsync(request.TargetAcademicYearId, currentAcademicYearId, cancellationToken);
            var classSectionGuid = Guid.Parse(request.ClassSectionId);
            var targetClassSection = await GetRequiredClassSectionAsync(Guid.Parse(request.TargetClassSectionId), cancellationToken);

            return await ExecuteStudentTransitionAsync(
                request,
                currentAcademicYearId,
                targetAcademicYear,
                classSectionGuid,
                targetClassSection,
                _ => true,
                markSourceAsPromoted: true,
                missingTargetMessage: string.Empty,
                cancellationToken);
        }

        private async Task<AcademicYear> ValidateAndGetTargetAcademicYearAsync(string targetAcademicYearId, Guid currentAcademicYearId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(targetAcademicYearId))
            {
                throw new Exception("Target academic year is required.");
            }

            var parsedTargetAcademicYearId = Guid.Parse(targetAcademicYearId);
            if (currentAcademicYearId == parsedTargetAcademicYearId)
            {
                throw new Exception("Target academic year must be different from the current session academic year.");
            }

            var targetAcademicYear = await _context.AcademicYears
                .FirstOrDefaultAsync(x => x.Id == parsedTargetAcademicYearId, cancellationToken);

            if (targetAcademicYear == null)
            {
                throw new Exception("Target academic year not found.");
            }

            return targetAcademicYear;
        }

        private async Task<ClassSection> GetRequiredClassSectionAsync(Guid classSectionId, CancellationToken cancellationToken)
        {
            var classSection = await _context.ClassSections
                .Include(x => x.ClassRoom)
                .Include(x => x.Section)
                .FirstOrDefaultAsync(x => x.Id == classSectionId, cancellationToken);

            if (classSection == null)
            {
                throw new Exception("Selected class section not found.");
            }

            return classSection;
        }

        private async Task<ClassSection?> ResolveDefaultPromotionTargetAsync(ClassSection currentClassSection, CancellationToken cancellationToken)
        {
            var nextClass = await _context.ClassRooms
                .FirstOrDefaultAsync(x => x.OrderNumber == currentClassSection.ClassRoom.OrderNumber + 1, cancellationToken);

            if (nextClass == null)
            {
                return null;
            }

            return await _context.ClassSections
                .Include(x => x.ClassRoom)
                .Include(x => x.Section)
                .FirstOrDefaultAsync(x => x.ClassId == nextClass.Id && x.SectionId == currentClassSection.SectionId, cancellationToken);
        }

        private async Task<PromotionExecutionResultViewModel> ExecuteStudentTransitionAsync(
            PromoteStudentsDto request,
            Guid currentAcademicYearId,
            AcademicYear targetAcademicYear,
            Guid sourceClassSectionId,
            ClassSection? targetClassSection,
            Func<StudentEnrollment, bool> eligibilityEvaluator,
            bool markSourceAsPromoted,
            string missingTargetMessage,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.ClassSectionId))
            {
                throw new Exception("Class section is required.");
            }

            if (!request.PromoteAllEligible && (request.StudentEnrollmentIds == null || !request.StudentEnrollmentIds.Any()))
            {
                throw new Exception("Select at least one student.");
            }

            var sourceEnrollmentIds = request.StudentEnrollmentIds?
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(Guid.Parse)
                .ToHashSet() ?? new HashSet<Guid>();

            var enrollmentQuery = _context.StudentEnrollments
                .Include(x => x.Student)
                .Include(x => x.ExamResults.Where(er => er.ExamType == request.ExamType))
                    .ThenInclude(er => er.SubjectMarks)
                .Where(x => x.AcademicYearId == currentAcademicYearId && x.ClassSectionId == sourceClassSectionId && x.Student.IsActive);

            if (!request.PromoteAllEligible)
            {
                enrollmentQuery = enrollmentQuery.Where(x => sourceEnrollmentIds.Contains(x.Id));
            }

            var enrollments = await enrollmentQuery
                .OrderBy(x => x.RollNumber ?? int.MaxValue)
                .ThenBy(x => x.Student.FirstName)
                .ThenBy(x => x.Student.LastName)
                .ToListAsync(cancellationToken);

            var result = new PromotionExecutionResultViewModel
            {
                ReviewedCount = enrollments.Count,
                TargetAcademicYearName = targetAcademicYear.YearName
            };

            if (!enrollments.Any())
            {
                return result;
            }

            var promotedTargetClassSections = new HashSet<Guid>();

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                foreach (var enrollment in enrollments)
                {
                    if (!eligibilityEvaluator(enrollment))
                    {
                        result.NotEligibleCount++;
                        result.SkippedCount++;
                        continue;
                    }

                    if (targetClassSection == null)
                    {
                        result.MissingTargetCount++;
                        result.SkippedCount++;
                        continue;
                    }

                    var alreadyEnrolled = await _context.StudentEnrollments
                        .AnyAsync(x => x.StudentId == enrollment.StudentId && x.AcademicYearId == targetAcademicYear.Id, cancellationToken);

                    if (alreadyEnrolled)
                    {
                        result.AlreadyEnrolledCount++;
                        result.SkippedCount++;
                        continue;
                    }

                    var newEnrollment = await CreateStudentEnrollmentAsync(
                        enrollment.StudentId,
                        targetClassSection.Id,
                        targetAcademicYear.Id,
                        false,
                        cancellationToken);

                    await MapStudentFeesAsync(newEnrollment, targetAcademicYear.Id, cancellationToken);

                    if (markSourceAsPromoted)
                    {
                        enrollment.IsPromoted = true;
                    }

                    result.PromotedCount++;
                    promotedTargetClassSections.Add(targetClassSection.Id);
                }

                await _context.SaveChangesAsync(cancellationToken);

                foreach (var targetClassSectionId in promotedTargetClassSections)
                {
                    await AssignRollNumbersInternalAsync(targetClassSectionId, targetAcademicYear.Id, cancellationToken);
                }

                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new Exception(ex.Message);
            }

            return result;
        }

        private static (string Status, bool IsPassed) EvaluatePromotionStatus(ExamResult? examResult)
        {
            if (examResult == null)
            {
                return ("No Result", false);
            }

            var grades = examResult.SubjectMarks
                .Select(x => (x.FinalGrade ?? string.Empty).Trim().ToUpperInvariant())
                .ToList();

            if (grades.Any(x => x == "NG"))
            {
                return ("NG", false);
            }

            if (grades.Any(x => x == "NQ"))
            {
                return ("NQ", false);
            }

            if (examResult.GPA < 1.6m)
            {
                return ("Failed", false);
            }

            return ("Passed", true);
        }

        private static string GetPromotionRemarks(string status, bool hasTarget, ClassRoom? nextClass, bool alreadyPromoted)
        {
            if (alreadyPromoted)
            {
                return "Student has already been promoted from this session.";
            }

            if (status == "No Result")
            {
                return "No result found for the selected terminal.";
            }

            if (status == "NG" || status == "NQ" || status == "Failed")
            {
                return "Student is not eligible for normal promotion.";
            }

            if (nextClass == null)
            {
                return "No next class configured for promotion.";
            }

            if (!hasTarget)
            {
                return "Next class exists, but matching section is not configured.";
            }

            return "Eligible for normal promotion.";
        }
        public async Task<List<StudentViewModel>> GetStudentByClassIdAsync(Guid classRooomId, CancellationToken cancellationToken)
        {
            var currentAcademicYearId = GetCurrentAcademicYearId();
            var students = await _context.StudentEnrollments.Include(x => x.ClassSection)
                                                   .ThenInclude(x => x.ClassRoom)
                                                  .Where(x => x.ClassSection.ClassId == classRooomId && x.AcademicYearId == currentAcademicYearId)
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
        public async Task<List<StudentViewModel>> GetStudentByClassSectionId(string classSectionId, int? examType, CancellationToken cancellationToken)
        {
            var classSectionGuid = Guid.Parse(classSectionId);
            var currentAcademicYearId = GetCurrentAcademicYearId();
            var studentQuery = _context.StudentEnrollments.Include(x => x.ClassSection)
                                                          .ThenInclude(x => x.ClassRoom)
                                                          .Where(x => x.Student.IsActive == true &&
                                                                 x.AcademicYearId == currentAcademicYearId &&
                                                                 x.ClassSection.Id == classSectionGuid);

            if (examType.HasValue)
            {
                studentQuery = studentQuery.Where(x => x.ExamResults.Any(e => e.ExamType == examType.Value));
            }

            var students = await studentQuery
                                                            .Select(x => new StudentViewModel
                                                            {
                                                                Id = x.StudentId,
                                                                StudentEnrollmentId = x.Id,
                                                                FirstName = x.Student.FirstName,
                                                                LastName = x.Student.LastName,
                                                                Address = x.Student.Municipality.MunicipalityName + " - " + x.Student.WardNo,
                                                                Age = x.Student.Age,
                                                                GrandFatherName = x.Student.GrandFatherName,
                                                                FatherName = x.Student.FatherName,
                                                                MotherName = x.Student.MotherName,
                                                                ClassRoomId = x.ClassSection.ClassRoom.Id.ToString(),
                                                                SectionId = x.ClassSection.Section.Id.ToString(),
                                                                ClassSectionId = x.ClassSection.Id.ToString(),
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
