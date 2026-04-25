using Application.Common.Interfaces;
using Application.Teachers;
using Domain;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class TeacherService : ITeacherService
    {
        private const string TeacherRoleName = "Teacher";
        private readonly IApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserResolver _userResolver;

        public TeacherService(
            IApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            UserResolver userResolver)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _userResolver = userResolver;
        }

        public async Task<List<TeacherViewModel>> GetTeachersAsync(string? academicYearId, bool includeInactive, CancellationToken cancellationToken)
        {
            var query = _context.Teachers
                .Include(x => x.TeacherClassSections.Where(a => !a.IsDeleted))
                    .ThenInclude(x => x.AcademicYear)
                .Include(x => x.TeacherClassSections.Where(a => !a.IsDeleted))
                    .ThenInclude(x => x.Course)
                .Include(x => x.TeacherClassSections.Where(a => !a.IsDeleted))
                    .ThenInclude(x => x.ClassSection)
                        .ThenInclude(x => x.ClassRoom)
                .Include(x => x.TeacherClassSections.Where(a => !a.IsDeleted))
                    .ThenInclude(x => x.ClassSection)
                        .ThenInclude(x => x.Section)
                .Include(x => x.Qualifications.Where(q => !q.IsDeleted))
                .Include(x => x.Experiences.Where(e => !e.IsDeleted))
                .Include(x => x.Documents.Where(d => !d.IsDeleted))
                .Where(x => !x.IsDeleted);

            if (!includeInactive)
            {
                query = query.Where(x => x.IsActive);
            }

            var teachers = await query
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToListAsync(cancellationToken);

            if (!string.IsNullOrWhiteSpace(academicYearId) && Guid.TryParse(academicYearId, out var academicYearGuid))
            {
                teachers.ForEach(teacher =>
                {
                    teacher.TeacherClassSections = teacher.TeacherClassSections
                        .Where(x => x.AcademicYearId == academicYearGuid)
                        .ToList();
                });
            }

            return teachers.Select(MapTeacher).ToList();
        }

        public async Task<TeacherViewModel?> GetTeacherByIdAsync(string teacherId, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(teacherId, out var teacherGuid))
            {
                throw new Exception("Invalid teacher id");
            }

            var teacher = await _context.Teachers
                .Include(x => x.TeacherClassSections.Where(a => !a.IsDeleted))
                    .ThenInclude(x => x.AcademicYear)
                .Include(x => x.TeacherClassSections.Where(a => !a.IsDeleted))
                    .ThenInclude(x => x.Course)
                .Include(x => x.TeacherClassSections.Where(a => !a.IsDeleted))
                    .ThenInclude(x => x.ClassSection)
                        .ThenInclude(x => x.ClassRoom)
                .Include(x => x.TeacherClassSections.Where(a => !a.IsDeleted))
                    .ThenInclude(x => x.ClassSection)
                        .ThenInclude(x => x.Section)
                .Include(x => x.Qualifications.Where(q => !q.IsDeleted))
                .Include(x => x.Experiences.Where(e => !e.IsDeleted))
                .Include(x => x.Documents.Where(d => !d.IsDeleted))
                .FirstOrDefaultAsync(x => x.Id == teacherGuid && !x.IsDeleted, cancellationToken);

            return teacher == null ? null : MapTeacher(teacher);
        }

        public async Task AddTeacherAsync(TeacherDto teacherDto, CancellationToken cancellationToken)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                teacherDto.EmployeeCode = string.IsNullOrWhiteSpace(teacherDto.EmployeeCode)
                    ? await GenerateEmployeeCodeAsync(cancellationToken)
                    : teacherDto.EmployeeCode.Trim();

                await ValidateTeacherAsync(teacherDto, null, cancellationToken);
                var userId = teacherDto.CreateUser ? await CreateTeacherUserAsync(teacherDto) : (Guid?)null;
                var teacher = new Teacher
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    EmployeeCode = teacherDto.EmployeeCode,
                    FirstName = teacherDto.FirstName.Trim(),
                    MiddleName = teacherDto.MiddleName ?? string.Empty,
                    LastName = teacherDto.LastName.Trim(),
                    Gender = teacherDto.Gender,
                    Age = teacherDto.Age,
                    DateOfBirthNp = teacherDto.DateOfBirthNp ?? string.Empty,
                    DateOfBirthEn = teacherDto.DateOfBirthEn ?? string.Empty,
                    ContactNumber = teacherDto.ContactNumber.Trim(),
                    AlternateContactNumber = teacherDto.AlternateContactNumber ?? string.Empty,
                    Email = teacherDto.Email ?? string.Empty,
                    ProvinceId = teacherDto.ProvinceId,
                    DistrictId = teacherDto.DistrictId,
                    MunicipalityId = teacherDto.MunicipalityId,
                    WardNo = teacherDto.WardNo,
                    FatherName = teacherDto.FatherName ?? string.Empty,
                    MotherName = teacherDto.MotherName ?? string.Empty,
                    Designation = teacherDto.Designation ?? string.Empty,
                    JoiningDateNp = teacherDto.JoiningDateNp ?? string.Empty,
                    JoiningDateEn = teacherDto.JoiningDateEn ?? string.Empty,
                    IsActive = true,
                    CreatedBy = GetCurrentUserId(),
                    ModifiedBy = GetCurrentUserId()
                };

                MapQualifications(teacher, teacherDto.Qualifications);
                MapExperiences(teacher, teacherDto.Experiences);
                await MapAssignmentsAsync(teacher, teacherDto.Assignments, cancellationToken);

                await _context.Teachers.AddAsync(teacher, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task UpdateTeacherAsync(TeacherDto teacherDto, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(teacherDto.Id, out var teacherGuid))
            {
                throw new Exception("Invalid teacher id");
            }

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var teacher = await _context.Teachers
                    .Include(x => x.Qualifications)
                    .Include(x => x.Experiences)
                    .Include(x => x.TeacherClassSections)
                    .FirstOrDefaultAsync(x => x.Id == teacherGuid && !x.IsDeleted, cancellationToken);

                if (teacher == null)
                {
                    throw new Exception("Teacher not found");
                }

                teacherDto.EmployeeCode = string.IsNullOrWhiteSpace(teacherDto.EmployeeCode)
                    ? teacher.EmployeeCode
                    : teacherDto.EmployeeCode.Trim();

                await ValidateTeacherAsync(teacherDto, teacherGuid, cancellationToken);
                if (teacherDto.CreateUser && teacher.UserId == null)
                {
                    teacher.UserId = await CreateTeacherUserAsync(teacherDto);
                }

                teacher.EmployeeCode = teacherDto.EmployeeCode;
                teacher.FirstName = teacherDto.FirstName.Trim();
                teacher.MiddleName = teacherDto.MiddleName ?? string.Empty;
                teacher.LastName = teacherDto.LastName.Trim();
                teacher.Gender = teacherDto.Gender;
                teacher.Age = teacherDto.Age;
                teacher.DateOfBirthNp = teacherDto.DateOfBirthNp ?? string.Empty;
                teacher.DateOfBirthEn = teacherDto.DateOfBirthEn ?? string.Empty;
                teacher.ContactNumber = teacherDto.ContactNumber.Trim();
                teacher.AlternateContactNumber = teacherDto.AlternateContactNumber ?? string.Empty;
                teacher.Email = teacherDto.Email ?? string.Empty;
                teacher.ProvinceId = teacherDto.ProvinceId;
                teacher.DistrictId = teacherDto.DistrictId;
                teacher.MunicipalityId = teacherDto.MunicipalityId;
                teacher.WardNo = teacherDto.WardNo;
                teacher.FatherName = teacherDto.FatherName ?? string.Empty;
                teacher.MotherName = teacherDto.MotherName ?? string.Empty;
                teacher.Designation = teacherDto.Designation ?? string.Empty;
                teacher.JoiningDateNp = teacherDto.JoiningDateNp ?? string.Empty;
                teacher.JoiningDateEn = teacherDto.JoiningDateEn ?? string.Empty;
                teacher.ModifiedBy = GetCurrentUserId();

                SyncQualifications(teacher, teacherDto.Qualifications);
                SyncExperiences(teacher, teacherDto.Experiences);
                await SyncAssignmentsAsync(teacher, teacherDto.Assignments, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task UpdateTeacherStatusAsync(string teacherId, TeacherStatusDto teacherStatusDto, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(teacherId, out var teacherGuid))
            {
                throw new Exception("Invalid teacher id");
            }

            var teacher = await _context.Teachers
                .Include(x => x.TeacherClassSections)
                .FirstOrDefaultAsync(x => x.Id == teacherGuid && !x.IsDeleted, cancellationToken);

            if (teacher == null)
            {
                throw new Exception("Teacher not found");
            }

            teacher.IsActive = teacherStatusDto.IsActive;
            teacher.InactiveReason = teacherStatusDto.IsActive ? string.Empty : teacherStatusDto.InactiveReason ?? string.Empty;
            teacher.InactiveDate = teacherStatusDto.IsActive ? null : DateTime.UtcNow;
            teacher.ModifiedBy = GetCurrentUserId();

            if (!teacherStatusDto.IsActive)
            {
                foreach (var assignment in teacher.TeacherClassSections.Where(x => !x.IsDeleted && x.IsActive))
                {
                    assignment.IsActive = false;
                    assignment.ModifiedBy = GetCurrentUserId();
                }
            }

            if (teacher.UserId.HasValue)
            {
                var user = await _userManager.FindByIdAsync(teacher.UserId.Value.ToString());
                if (user != null)
                {
                    user.IsActive = teacherStatusDto.IsActive;
                    await _userManager.UpdateAsync(user);
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task SoftDeleteTeacherAsync(string teacherId, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(teacherId, out var teacherGuid))
            {
                throw new Exception("Invalid teacher id");
            }

            var teacher = await _context.Teachers
                .Include(x => x.TeacherClassSections)
                .FirstOrDefaultAsync(x => x.Id == teacherGuid && !x.IsDeleted, cancellationToken);

            if (teacher == null)
            {
                throw new Exception("Teacher not found");
            }

            teacher.IsDeleted = true;
            teacher.IsActive = false;
            teacher.DeletedOn = DateTime.UtcNow;
            teacher.DeletedBy = GetCurrentUserId();

            foreach (var assignment in teacher.TeacherClassSections.Where(x => !x.IsDeleted))
            {
                assignment.IsDeleted = true;
                assignment.IsActive = false;
                assignment.DeletedOn = DateTime.UtcNow;
                assignment.DeletedBy = GetCurrentUserId();
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<TeacherDocumentViewModel> AddTeacherDocumentAsync(TeacherDocumentDto teacherDocumentDto, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(teacherDocumentDto.TeacherId, out var teacherId))
            {
                throw new Exception("Invalid teacher id");
            }

            var teacherExists = await _context.Teachers.AnyAsync(x => x.Id == teacherId && !x.IsDeleted, cancellationToken);
            if (!teacherExists)
            {
                throw new Exception("Teacher not found");
            }

            var document = new TeacherDocument
            {
                Id = Guid.NewGuid(),
                TeacherId = teacherId,
                DocumentType = teacherDocumentDto.DocumentType,
                DocumentTitle = teacherDocumentDto.DocumentTitle,
                FilePath = teacherDocumentDto.FilePath,
                OriginalFileName = teacherDocumentDto.OriginalFileName,
                MimeType = teacherDocumentDto.MimeType,
                FileSize = teacherDocumentDto.FileSize,
                UploadedDate = DateTime.UtcNow,
                IsActive = true,
                CreatedBy = GetCurrentUserId(),
                ModifiedBy = GetCurrentUserId()
            };

            await _context.TeacherDocuments.AddAsync(document, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return MapDocument(document);
        }

        public async Task<TeacherDocumentViewModel?> GetTeacherDocumentAsync(string documentId, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(documentId, out var documentGuid))
            {
                throw new Exception("Invalid document id");
            }

            var document = await _context.TeacherDocuments.FirstOrDefaultAsync(x => x.Id == documentGuid && !x.IsDeleted, cancellationToken);
            return document == null ? null : MapDocument(document);
        }

        public async Task DeleteTeacherDocumentAsync(string documentId, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(documentId, out var documentGuid))
            {
                throw new Exception("Invalid document id");
            }

            var document = await _context.TeacherDocuments.FirstOrDefaultAsync(x => x.Id == documentGuid && !x.IsDeleted, cancellationToken);
            if (document == null)
            {
                throw new Exception("Document not found");
            }

            document.IsDeleted = true;
            document.IsActive = false;
            document.DeletedOn = DateTime.UtcNow;
            document.DeletedBy = GetCurrentUserId();
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<TeacherDashboardViewModel> GetTeacherDashboardAsync(string? academicYearId, CancellationToken cancellationToken)
        {
            Guid? academicYearGuid = Guid.TryParse(academicYearId, out var parsedAcademicYear) ? parsedAcademicYear : null;
            var assignments = _context.TeacherClassSections.Where(x => !x.IsDeleted && x.IsActive);
            if (academicYearGuid.HasValue)
            {
                assignments = assignments.Where(x => x.AcademicYearId == academicYearGuid.Value);
            }

            return new TeacherDashboardViewModel
            {
                TotalActiveTeachers = await _context.Teachers.CountAsync(x => !x.IsDeleted && x.IsActive, cancellationToken),
                TotalInactiveTeachers = await _context.Teachers.CountAsync(x => !x.IsDeleted && !x.IsActive, cancellationToken),
                AssignedTeachers = await assignments.Select(x => x.TeacherId).Distinct().CountAsync(cancellationToken),
                ClassTeachers = await assignments.Where(x => x.IsClassTeacher).Select(x => x.TeacherId).Distinct().CountAsync(cancellationToken)
            };
        }

        private async Task ValidateTeacherAsync(TeacherDto teacherDto, Guid? teacherId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(teacherDto.FirstName) || string.IsNullOrWhiteSpace(teacherDto.LastName))
            {
                throw new Exception("Teacher first name and last name are required");
            }
            if (teacherDto.Gender <= 0)
            {
                throw new Exception("Teacher gender is required");
            }
            if (string.IsNullOrWhiteSpace(teacherDto.ContactNumber))
            {
                throw new Exception("Teacher contact number is required");
            }
            if (!string.IsNullOrWhiteSpace(teacherDto.EmployeeCode))
            {
                var employeeExists = await _context.Teachers.AnyAsync(x => x.EmployeeCode == teacherDto.EmployeeCode && !x.IsDeleted && (!teacherId.HasValue || x.Id != teacherId.Value), cancellationToken);
                if (employeeExists)
                {
                    throw new Exception("Employee code already exists");
                }
            }
            if (!string.IsNullOrWhiteSpace(teacherDto.Email))
            {
                var emailExists = await _context.Teachers.AnyAsync(x => x.Email == teacherDto.Email && !x.IsDeleted && (!teacherId.HasValue || x.Id != teacherId.Value), cancellationToken);
                if (emailExists)
                {
                    throw new Exception("Teacher email already exists");
                }
            }
            var existingTeacher = teacherId.HasValue
                ? await _context.Teachers.FirstOrDefaultAsync(x => x.Id == teacherId.Value && !x.IsDeleted, cancellationToken)
                : null;

            if (teacherDto.CreateUser && (teacherId == null || existingTeacher?.UserId == null))
            {
                if (string.IsNullOrWhiteSpace(teacherDto.UserName) || string.IsNullOrWhiteSpace(teacherDto.Password))
                {
                    throw new Exception("Username and password are required when creating teacher user");
                }
                var userExists = await _userManager.FindByNameAsync(teacherDto.UserName);
                if (userExists != null && userExists.Id != existingTeacher?.UserId)
                {
                    throw new Exception("Username already exists");
                }
            }

            ValidateDuplicatePayloadAssignments(teacherDto.Assignments);
        }

        private async Task<string> GenerateEmployeeCodeAsync(CancellationToken cancellationToken)
        {
            const string prefix = "TCH";
            var existingCodes = await _context.Teachers
                .Where(x => !x.IsDeleted && x.EmployeeCode.StartsWith(prefix))
                .Select(x => x.EmployeeCode)
                .ToListAsync(cancellationToken);

            var nextNumber = existingCodes
                .Select(code => int.TryParse(code.Replace(prefix, string.Empty), out var number) ? number : 0)
                .DefaultIfEmpty(0)
                .Max() + 1;

            return $"{prefix}{nextNumber:0000}";
        }

        private static void ValidateDuplicatePayloadAssignments(List<TeacherClassSectionDto> assignments)
        {
            var duplicateSubjectAssignment = assignments
                .GroupBy(x => new { x.AcademicYearId, x.ClassSectionId, x.CourseId })
                .FirstOrDefault(x => x.Count() > 1);
            if (duplicateSubjectAssignment != null)
            {
                throw new Exception("Duplicate class, section, and course assignment found");
            }

            var duplicateClassTeacher = assignments
                .Where(x => x.IsClassTeacher)
                .GroupBy(x => new { x.AcademicYearId, x.ClassSectionId })
                .FirstOrDefault(x => x.Count() > 1);
            if (duplicateClassTeacher != null)
            {
                throw new Exception("Only one class teacher assignment is allowed for a class section");
            }
        }

        private async Task<Guid> CreateTeacherUserAsync(TeacherDto teacherDto)
        {
            if (!await _roleManager.RoleExistsAsync(TeacherRoleName))
            {
                await _roleManager.CreateAsync(new ApplicationRole { Name = TeacherRoleName, Description = "Teacher role" });
            }

            var user = new ApplicationUser
            {
                UserName = teacherDto.UserName,
                Email = teacherDto.Email,
                PhoneNumber = teacherDto.ContactNumber,
                FirstName = teacherDto.FirstName,
                LastName = teacherDto.LastName,
                ShortName = $"{teacherDto.FirstName.FirstOrDefault()}{teacherDto.LastName.FirstOrDefault()}".ToUpper(),
                Address = BuildTeacherAddress(teacherDto),
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, teacherDto.Password);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(x => x.Description)));
            }

            var roleResult = await _userManager.AddToRoleAsync(user, TeacherRoleName);
            if (!roleResult.Succeeded)
            {
                throw new Exception(string.Join(", ", roleResult.Errors.Select(x => x.Description)));
            }

            return user.Id;
        }

        private static string BuildTeacherAddress(TeacherDto teacherDto)
        {
            var addressParts = new List<string>();
            if (teacherDto.ProvinceId.HasValue)
            {
                addressParts.Add($"Province {teacherDto.ProvinceId.Value}");
            }
            if (teacherDto.DistrictId.HasValue)
            {
                addressParts.Add($"District {teacherDto.DistrictId.Value}");
            }
            if (teacherDto.MunicipalityId.HasValue)
            {
                addressParts.Add($"Municipality {teacherDto.MunicipalityId.Value}");
            }
            if (teacherDto.WardNo.HasValue)
            {
                addressParts.Add($"Ward {teacherDto.WardNo.Value}");
            }

            return string.Join(", ", addressParts);
        }

        private static void MapQualifications(Teacher teacher, List<TeacherQualificationDto> qualifications)
        {
            foreach (var qualification in qualifications)
            {
                teacher.Qualifications.Add(new TeacherQualification
                {
                    Id = Guid.NewGuid(),
                    DegreeName = qualification.DegreeName,
                    InstitutionName = qualification.InstitutionName,
                    BoardOrUniversity = qualification.BoardOrUniversity ?? string.Empty,
                    PassedYear = qualification.PassedYear ?? string.Empty,
                    GradeOrPercentage = qualification.GradeOrPercentage ?? string.Empty,
                    MajorSubject = qualification.MajorSubject ?? string.Empty,
                    Remarks = qualification.Remarks ?? string.Empty,
                    IsActive = true
                });
            }
        }

        private static void MapExperiences(Teacher teacher, List<TeacherExperienceDto> experiences)
        {
            foreach (var experience in experiences)
            {
                teacher.Experiences.Add(new TeacherExperience
                {
                    Id = Guid.NewGuid(),
                    OrganizationName = experience.OrganizationName,
                    Designation = experience.Designation,
                    SubjectOrDepartment = experience.SubjectOrDepartment ?? string.Empty,
                    StartDateNp = experience.StartDateNp ?? string.Empty,
                    StartDateEn = experience.StartDateEn ?? string.Empty,
                    EndDateNp = experience.EndDateNp ?? string.Empty,
                    EndDateEn = experience.EndDateEn ?? string.Empty,
                    IsCurrent = experience.IsCurrent,
                    Remarks = experience.Remarks ?? string.Empty,
                    IsActive = true
                });
            }
        }

        private async Task MapAssignmentsAsync(Teacher teacher, List<TeacherClassSectionDto> assignments, CancellationToken cancellationToken)
        {
            foreach (var assignment in assignments)
            {
                await ValidateAssignmentAsync(teacher.Id, assignment, null, cancellationToken);
                teacher.TeacherClassSections.Add(new TeacherClassSection
                {
                    Id = Guid.NewGuid(),
                    AcademicYearId = Guid.Parse(assignment.AcademicYearId),
                    ClassSectionId = Guid.Parse(assignment.ClassSectionId),
                    CourseId = Guid.Parse(assignment.CourseId),
                    IsClassTeacher = assignment.IsClassTeacher,
                    Remarks = assignment.Remarks ?? string.Empty,
                    IsActive = true
                });
            }
        }

        private void SyncQualifications(Teacher teacher, List<TeacherQualificationDto> qualifications)
        {
            SyncChildCollection(
                teacher.Qualifications,
                qualifications,
                x => x.Id,
                x => new TeacherQualification
                {
                    Id = Guid.NewGuid(),
                    TeacherId = teacher.Id,
                    IsActive = true
                },
                (entity, dto) =>
                {
                    entity.DegreeName = dto.DegreeName;
                    entity.InstitutionName = dto.InstitutionName;
                    entity.BoardOrUniversity = dto.BoardOrUniversity ?? string.Empty;
                    entity.PassedYear = dto.PassedYear ?? string.Empty;
                    entity.GradeOrPercentage = dto.GradeOrPercentage ?? string.Empty;
                    entity.MajorSubject = dto.MajorSubject ?? string.Empty;
                    entity.Remarks = dto.Remarks ?? string.Empty;
                });
        }

        private void SyncExperiences(Teacher teacher, List<TeacherExperienceDto> experiences)
        {
            SyncChildCollection(
                teacher.Experiences,
                experiences,
                x => x.Id,
                x => new TeacherExperience
                {
                    Id = Guid.NewGuid(),
                    TeacherId = teacher.Id,
                    IsActive = true
                },
                (entity, dto) =>
                {
                    entity.OrganizationName = dto.OrganizationName;
                    entity.Designation = dto.Designation;
                    entity.SubjectOrDepartment = dto.SubjectOrDepartment ?? string.Empty;
                    entity.StartDateNp = dto.StartDateNp ?? string.Empty;
                    entity.StartDateEn = dto.StartDateEn ?? string.Empty;
                    entity.EndDateNp = dto.EndDateNp ?? string.Empty;
                    entity.EndDateEn = dto.EndDateEn ?? string.Empty;
                    entity.IsCurrent = dto.IsCurrent;
                    entity.Remarks = dto.Remarks ?? string.Empty;
                });
        }

        private async Task SyncAssignmentsAsync(Teacher teacher, List<TeacherClassSectionDto> assignments, CancellationToken cancellationToken)
        {
            var submittedIds = assignments
                .Where(x => Guid.TryParse(x.Id, out _))
                .Select(x => Guid.Parse(x.Id!))
                .ToHashSet();

            foreach (var existing in teacher.TeacherClassSections.Where(x => !x.IsDeleted && !submittedIds.Contains(x.Id)))
            {
                existing.IsDeleted = true;
                existing.IsActive = false;
                existing.DeletedOn = DateTime.UtcNow;
                existing.DeletedBy = GetCurrentUserId();
            }

            foreach (var assignment in assignments)
            {
                var assignmentId = Guid.TryParse(assignment.Id, out var parsedId) ? parsedId : Guid.Empty;
                var entity = teacher.TeacherClassSections.FirstOrDefault(x => x.Id == assignmentId);
                await ValidateAssignmentAsync(teacher.Id, assignment, assignmentId == Guid.Empty ? null : assignmentId, cancellationToken);

                if (entity == null)
                {
                    entity = new TeacherClassSection
                    {
                        Id = Guid.NewGuid(),
                        TeacherId = teacher.Id
                    };
                    teacher.TeacherClassSections.Add(entity);
                }

                entity.AcademicYearId = Guid.Parse(assignment.AcademicYearId);
                entity.ClassSectionId = Guid.Parse(assignment.ClassSectionId);
                entity.CourseId = Guid.Parse(assignment.CourseId);
                entity.IsClassTeacher = assignment.IsClassTeacher;
                entity.Remarks = assignment.Remarks ?? string.Empty;
                entity.IsActive = true;
                entity.IsDeleted = false;
                entity.ModifiedBy = GetCurrentUserId();
            }
        }

        private async Task ValidateAssignmentAsync(Guid teacherId, TeacherClassSectionDto assignment, Guid? assignmentId, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(assignment.AcademicYearId, out var academicYearId) ||
                !Guid.TryParse(assignment.ClassSectionId, out var classSectionId) ||
                !Guid.TryParse(assignment.CourseId, out var courseId))
            {
                throw new Exception("Invalid teacher assignment");
            }

            var classSectionExists = await _context.ClassSections.AnyAsync(x => x.Id == classSectionId, cancellationToken);
            var courseExists = await _context.Courses.AnyAsync(x => x.Id == courseId && !x.IsDeleted, cancellationToken);
            var academicYearExists = await _context.AcademicYears.AnyAsync(x => x.Id == academicYearId && !x.IsDeleted, cancellationToken);
            if (!classSectionExists || !courseExists || !academicYearExists)
            {
                throw new Exception("Selected academic year, class section, or course does not exist");
            }

            var duplicateTeacherAssignment = await _context.TeacherClassSections.AnyAsync(x =>
                !x.IsDeleted &&
                (!assignmentId.HasValue || x.Id != assignmentId.Value) &&
                x.TeacherId == teacherId &&
                x.AcademicYearId == academicYearId &&
                x.ClassSectionId == classSectionId &&
                x.CourseId == courseId,
                cancellationToken);
            if (duplicateTeacherAssignment)
            {
                throw new Exception("Teacher is already assigned to this class section and course");
            }

            var duplicateSubjectAssignment = await _context.TeacherClassSections.AnyAsync(x =>
                !x.IsDeleted &&
                x.IsActive &&
                (!assignmentId.HasValue || x.Id != assignmentId.Value) &&
                x.AcademicYearId == academicYearId &&
                x.ClassSectionId == classSectionId &&
                x.CourseId == courseId,
                cancellationToken);
            if (duplicateSubjectAssignment)
            {
                throw new Exception("This course is already assigned to another teacher for the selected class section");
            }

            if (assignment.IsClassTeacher)
            {
                var classAlreadyHasTeacher = await _context.TeacherClassSections.AnyAsync(x =>
                    !x.IsDeleted &&
                    x.IsActive &&
                    x.IsClassTeacher &&
                    (!assignmentId.HasValue || x.Id != assignmentId.Value) &&
                    x.AcademicYearId == academicYearId &&
                    x.ClassSectionId == classSectionId,
                    cancellationToken);
                if (classAlreadyHasTeacher)
                {
                    throw new Exception("This class section already has a class teacher");
                }

                var teacherAlreadyClassTeacher = await _context.TeacherClassSections.AnyAsync(x =>
                    !x.IsDeleted &&
                    x.IsActive &&
                    x.IsClassTeacher &&
                    (!assignmentId.HasValue || x.Id != assignmentId.Value) &&
                    x.TeacherId == teacherId &&
                    x.AcademicYearId == academicYearId,
                    cancellationToken);
                if (teacherAlreadyClassTeacher)
                {
                    throw new Exception("Teacher is already class teacher for another class section in this academic year");
                }
            }
        }

        private void SyncChildCollection<TEntity, TDto>(
            ICollection<TEntity> entities,
            List<TDto> dtos,
            Func<TDto, string?> getDtoId,
            Func<TDto, TEntity> createEntity,
            Action<TEntity, TDto> map)
            where TEntity : AuditableEntry
        {
            var submittedIds = dtos
                .Where(x => Guid.TryParse(getDtoId(x), out _))
                .Select(x => Guid.Parse(getDtoId(x)!))
                .ToHashSet();

            foreach (var existing in entities.Where(x => !x.IsDeleted && !submittedIds.Contains(x.Id)))
            {
                existing.IsDeleted = true;
                existing.IsActive = false;
                existing.DeletedOn = DateTime.UtcNow;
                existing.DeletedBy = GetCurrentUserId();
            }

            foreach (var dto in dtos)
            {
                var dtoId = Guid.TryParse(getDtoId(dto), out var parsedId) ? parsedId : Guid.Empty;
                var entity = entities.FirstOrDefault(x => x.Id == dtoId);
                if (entity == null)
                {
                    entity = createEntity(dto);
                    entities.Add(entity);
                }

                map(entity, dto);
                entity.IsActive = true;
                entity.IsDeleted = false;
                entity.ModifiedBy = GetCurrentUserId();
            }
        }

        private static TeacherViewModel MapTeacher(Teacher teacher)
        {
            var fullName = string.Join(" ", new[] { teacher.FirstName, teacher.MiddleName, teacher.LastName }.Where(x => !string.IsNullOrWhiteSpace(x)));
            return new TeacherViewModel
            {
                Id = teacher.Id,
                UserId = teacher.UserId,
                EmployeeCode = teacher.EmployeeCode,
                FirstName = teacher.FirstName,
                MiddleName = teacher.MiddleName,
                LastName = teacher.LastName,
                FullName = fullName,
                Gender = teacher.Gender,
                Age = teacher.Age,
                DateOfBirthNp = teacher.DateOfBirthNp,
                DateOfBirthEn = teacher.DateOfBirthEn,
                ContactNumber = teacher.ContactNumber,
                AlternateContactNumber = teacher.AlternateContactNumber,
                Email = teacher.Email,
                ProvinceId = teacher.ProvinceId,
                DistrictId = teacher.DistrictId,
                MunicipalityId = teacher.MunicipalityId,
                WardNo = teacher.WardNo,
                FatherName = teacher.FatherName,
                MotherName = teacher.MotherName,
                Designation = teacher.Designation,
                JoiningDateNp = teacher.JoiningDateNp,
                JoiningDateEn = teacher.JoiningDateEn,
                IsActive = teacher.IsActive,
                InactiveReason = teacher.InactiveReason,
                Qualifications = teacher.Qualifications.Where(x => !x.IsDeleted).Select(x => new TeacherQualificationViewModel
                {
                    Id = x.Id,
                    DegreeName = x.DegreeName,
                    InstitutionName = x.InstitutionName,
                    BoardOrUniversity = x.BoardOrUniversity,
                    PassedYear = x.PassedYear,
                    GradeOrPercentage = x.GradeOrPercentage,
                    MajorSubject = x.MajorSubject,
                    Remarks = x.Remarks
                }).ToList(),
                Experiences = teacher.Experiences.Where(x => !x.IsDeleted).Select(x => new TeacherExperienceViewModel
                {
                    Id = x.Id,
                    OrganizationName = x.OrganizationName,
                    Designation = x.Designation,
                    SubjectOrDepartment = x.SubjectOrDepartment,
                    StartDateNp = x.StartDateNp,
                    StartDateEn = x.StartDateEn,
                    EndDateNp = x.EndDateNp,
                    EndDateEn = x.EndDateEn,
                    IsCurrent = x.IsCurrent,
                    Remarks = x.Remarks
                }).ToList(),
                Documents = teacher.Documents.Where(x => !x.IsDeleted).Select(MapDocument).ToList(),
                Assignments = teacher.TeacherClassSections.Where(x => !x.IsDeleted).Select(x => new TeacherClassSectionViewModel
                {
                    Id = x.Id,
                    AcademicYearId = x.AcademicYearId,
                    AcademicYearName = x.AcademicYear?.YearName ?? string.Empty,
                    ClassSectionId = x.ClassSectionId,
                    ClassRoomId = x.ClassSection?.ClassId ?? Guid.Empty,
                    ClassRoomName = x.ClassSection?.ClassRoom?.Name ?? string.Empty,
                    SectionId = x.ClassSection?.SectionId ?? Guid.Empty,
                    SectionName = x.ClassSection?.Section?.Name ?? string.Empty,
                    CourseId = x.CourseId,
                    CourseName = x.Course?.Name ?? string.Empty,
                    IsClassTeacher = x.IsClassTeacher,
                    IsActive = x.IsActive,
                    Remarks = x.Remarks
                }).ToList()
            };
        }

        private static TeacherDocumentViewModel MapDocument(TeacherDocument document)
        {
            return new TeacherDocumentViewModel
            {
                Id = document.Id,
                DocumentType = document.DocumentType,
                DocumentTitle = document.DocumentTitle,
                FilePath = document.FilePath,
                OriginalFileName = document.OriginalFileName,
                MimeType = document.MimeType,
                FileSize = document.FileSize,
                UploadedDate = document.UploadedDate
            };
        }

        private Guid GetCurrentUserId()
        {
            return Guid.TryParse(_userResolver.UserId, out var userId) ? userId : Guid.Empty;
        }
    }
}
