using Application.Common.Interfaces;
using Application.Guardians.Dtos;
using Application.Guardians.Interfaces;
using Application.Guardians.ViewModels;
using Domain;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Guardians
{
    public class GuardianService : IGuardianService
    {
        private const string GuardianRoleName = "Guardian";
        private readonly IApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserResolver _userResolver;

        public GuardianService(
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

        public async Task<List<StudentGuardianViewModel>> GetStudentGuardiansAsync(string studentId, CancellationToken cancellationToken)
        {
            var studentGuid = ParseGuid(studentId, "Invalid student id");
            await EnsureStudentExistsAsync(studentGuid, cancellationToken);

            var links = await _context.GuardianStudents
                .AsNoTracking()
                .Include(x => x.Guardian)
                .Where(x => x.StudentId == studentGuid && !x.IsDeleted && !x.Guardian.IsDeleted)
                .OrderByDescending(x => x.IsPrimaryGuardian)
                .ThenBy(x => x.Guardian.FullName)
                .ToListAsync(cancellationToken);

            return await MapStudentGuardianLinksAsync(links);
        }

        public async Task<List<GuardianViewModel>> SearchGuardiansAsync(string? keyword, CancellationToken cancellationToken)
        {
            var cleanKeyword = keyword?.Trim().ToLower();
            var query = _context.Guardians
                .AsNoTracking()
                .Include(x => x.GuardianStudents)
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(cleanKeyword))
            {
                query = query.Where(x =>
                    x.FullName.ToLower().Contains(cleanKeyword) ||
                    x.ContactNumber.ToLower().Contains(cleanKeyword) ||
                    (x.Email != null && x.Email.ToLower().Contains(cleanKeyword)));
            }

            var guardians = await query
                .OrderBy(x => x.FullName)
                .Take(25)
                .ToListAsync(cancellationToken);

            var result = new List<GuardianViewModel>();
            foreach (var guardian in guardians)
            {
                var user = await _userManager.FindByIdAsync(guardian.UserId.ToString());
                result.Add(new GuardianViewModel
                {
                    Id = guardian.Id,
                    UserId = guardian.UserId,
                    FullName = guardian.FullName,
                    ContactNumber = guardian.ContactNumber,
                    Email = guardian.Email,
                    RelationType = guardian.RelationType,
                    UserName = user?.UserName ?? string.Empty,
                    IsActive = guardian.IsActive && (user?.IsActive ?? false),
                    LinkedStudentsCount = guardian.GuardianStudents.Count(x => !x.IsDeleted)
                });
            }

            return result;
        }

        public async Task<StudentGuardianViewModel> CreateGuardianForStudentAsync(string studentId, GuardianCreateDto guardianCreateDto, CancellationToken cancellationToken)
        {
            var studentGuid = ParseGuid(studentId, "Invalid student id");
            await ValidateCreateGuardianRequestAsync(studentGuid, guardianCreateDto, cancellationToken);

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await EnsureGuardianRoleAsync();
                var user = await CreateGuardianUserAsync(guardianCreateDto);

                var guardian = new Guardian
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    FullName = guardianCreateDto.FullName.Trim(),
                    ContactNumber = guardianCreateDto.ContactNumber.Trim(),
                    Email = CleanNullable(guardianCreateDto.Email),
                    RelationType = guardianCreateDto.RelationType.Trim(),
                    IsActive = true,
                    CreatedBy = GetCurrentUserId(),
                    ModifiedBy = GetCurrentUserId()
                };

                await _context.Guardians.AddAsync(guardian, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                var link = await CreateGuardianStudentLinkAsync(
                    guardian.Id,
                    studentGuid,
                    guardianCreateDto.IsPrimaryGuardian,
                    guardianCreateDto.CanViewFees,
                    guardianCreateDto.CanViewResults,
                    guardianCreateDto.CanViewAttendance,
                    guardianCreateDto.CanPayFees,
                    cancellationToken);

                await transaction.CommitAsync(cancellationToken);
                return (await MapStudentGuardianLinksAsync([link])).First();
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task<StudentGuardianViewModel> LinkGuardianToStudentAsync(GuardianStudentLinkDto guardianStudentLinkDto, CancellationToken cancellationToken)
        {
            var guardianGuid = ParseGuid(guardianStudentLinkDto.GuardianId, "Invalid guardian id");
            var studentGuid = ParseGuid(guardianStudentLinkDto.StudentId, "Invalid student id");

            var guardian = await _context.Guardians.FirstOrDefaultAsync(x => x.Id == guardianGuid && !x.IsDeleted, cancellationToken);
            if (guardian == null)
            {
                throw new Exception("Guardian not found");
            }

            await EnsureStudentExistsAsync(studentGuid, cancellationToken);
            var existingLink = await _context.GuardianStudents
                .FirstOrDefaultAsync(x => x.GuardianId == guardianGuid && x.StudentId == studentGuid && !x.IsDeleted, cancellationToken);
            if (existingLink != null)
            {
                throw new Exception("This guardian is already linked to the selected student");
            }

            await EnsureGuardianRoleAsync();
            var user = await _userManager.FindByIdAsync(guardian.UserId.ToString());
            if (user == null)
            {
                throw new Exception("Guardian user account not found");
            }
            if (!await _userManager.IsInRoleAsync(user, GuardianRoleName))
            {
                var roleResult = await _userManager.AddToRoleAsync(user, GuardianRoleName);
                EnsureIdentityResult(roleResult, "Failed to assign guardian role");
            }

            var link = await CreateGuardianStudentLinkAsync(
                guardianGuid,
                studentGuid,
                guardianStudentLinkDto.IsPrimaryGuardian,
                guardianStudentLinkDto.CanViewFees,
                guardianStudentLinkDto.CanViewResults,
                guardianStudentLinkDto.CanViewAttendance,
                guardianStudentLinkDto.CanPayFees,
                cancellationToken);

            return (await MapStudentGuardianLinksAsync([link])).First();
        }

        public async Task UpdateStudentGuardianAccessAsync(string guardianStudentId, GuardianStudentAccessDto guardianStudentAccessDto, CancellationToken cancellationToken)
        {
            var guardianStudentGuid = ParseGuid(guardianStudentId, "Invalid guardian student id");
            var link = await _context.GuardianStudents
                .FirstOrDefaultAsync(x => x.Id == guardianStudentGuid && !x.IsDeleted, cancellationToken);
            if (link == null)
            {
                throw new Exception("Guardian student link not found");
            }

            if (guardianStudentAccessDto.IsPrimaryGuardian)
            {
                await ClearOtherPrimaryGuardiansAsync(link.StudentId, link.Id, cancellationToken);
            }

            link.IsPrimaryGuardian = guardianStudentAccessDto.IsPrimaryGuardian;
            link.CanViewFees = guardianStudentAccessDto.CanViewFees;
            link.CanViewResults = guardianStudentAccessDto.CanViewResults;
            link.CanViewAttendance = guardianStudentAccessDto.CanViewAttendance;
            link.CanPayFees = guardianStudentAccessDto.CanPayFees;
            link.ModifiedBy = GetCurrentUserId();

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UnlinkGuardianFromStudentAsync(string guardianStudentId, CancellationToken cancellationToken)
        {
            var guardianStudentGuid = ParseGuid(guardianStudentId, "Invalid guardian student id");
            var link = await _context.GuardianStudents
                .FirstOrDefaultAsync(x => x.Id == guardianStudentGuid && !x.IsDeleted, cancellationToken);
            if (link == null)
            {
                throw new Exception("Guardian student link not found");
            }

            link.IsDeleted = true;
            link.IsActive = false;
            link.DeletedOn = DateTime.UtcNow;
            link.DeletedBy = GetCurrentUserId();
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<GuardianLinkedStudentViewModel>> GetCurrentGuardianStudentsAsync(CancellationToken cancellationToken)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
            {
                throw new Exception("Current user is not available");
            }

            return await _context.GuardianStudents
                .AsNoTracking()
                .Include(x => x.Guardian)
                .Include(x => x.Student)
                    .ThenInclude(x => x.StudentEnrollments)
                    .ThenInclude(x => x.ClassSection)
                    .ThenInclude(x => x.ClassRoom)
                .Include(x => x.Student)
                    .ThenInclude(x => x.StudentEnrollments)
                    .ThenInclude(x => x.ClassSection)
                    .ThenInclude(x => x.Section)
                .Where(x => !x.IsDeleted &&
                            x.IsActive &&
                            x.Guardian.UserId == currentUserId &&
                            x.Guardian.IsActive &&
                            !x.Guardian.IsDeleted)
                .OrderBy(x => x.Student.FirstName)
                .ThenBy(x => x.Student.LastName)
                .Select(x => new GuardianLinkedStudentViewModel
                {
                    StudentId = x.StudentId,
                    StudentEnrollmentId = x.Student.StudentEnrollments
                        .Where(e => e.IsActive && !e.IsDeleted)
                        .OrderByDescending(e => e.CreatedDate)
                        .Select(e => (Guid?)e.Id)
                        .FirstOrDefault(),
                    AcademicYearId = x.Student.StudentEnrollments
                        .Where(e => e.IsActive && !e.IsDeleted)
                        .OrderByDescending(e => e.CreatedDate)
                        .Select(e => (Guid?)e.AcademicYearId)
                        .FirstOrDefault(),
                    ClassSectionId = x.Student.StudentEnrollments
                        .Where(e => e.IsActive && !e.IsDeleted)
                        .OrderByDescending(e => e.CreatedDate)
                        .Select(e => (Guid?)e.ClassSectionId)
                        .FirstOrDefault(),
                    GuardianStudentId = x.Id,
                    StudentName = (x.Student.FirstName + " " + x.Student.LastName).Trim(),
                    ClassName = x.Student.StudentEnrollments
                        .Where(e => e.IsActive && !e.IsDeleted)
                        .OrderByDescending(e => e.CreatedDate)
                        .Select(e => e.ClassSection.ClassRoom.Name)
                        .FirstOrDefault() ?? string.Empty,
                    SectionName = x.Student.StudentEnrollments
                        .Where(e => e.IsActive && !e.IsDeleted)
                        .OrderByDescending(e => e.CreatedDate)
                        .Select(e => e.ClassSection.Section.Name)
                        .FirstOrDefault() ?? string.Empty,
                    RollNumber = x.Student.StudentEnrollments
                        .Where(e => e.IsActive && !e.IsDeleted)
                        .OrderByDescending(e => e.CreatedDate)
                        .Select(e => e.RollNumber)
                        .FirstOrDefault(),
                    CanViewFees = x.CanViewFees,
                    CanViewResults = x.CanViewResults,
                    CanViewAttendance = x.CanViewAttendance,
                    CanPayFees = x.CanPayFees
                })
                .ToListAsync(cancellationToken);
        }

        public async Task EnsureCurrentGuardianCanViewFeesAsync(string studentEnrollmentId, CancellationToken cancellationToken)
        {
            var enrollmentGuid = ParseGuid(studentEnrollmentId, "Invalid student enrollment id");
            var allowed = await CurrentGuardianLinks()
                .AnyAsync(x =>
                    x.CanViewFees &&
                    x.Student.StudentEnrollments.Any(e => e.Id == enrollmentGuid && !e.IsDeleted),
                    cancellationToken);

            if (!allowed)
            {
                throw new UnauthorizedAccessException("You are not allowed to view fees for this student.");
            }
        }

        public async Task EnsureCurrentGuardianCanViewAttendanceAsync(string studentId, CancellationToken cancellationToken)
        {
            var studentGuid = ParseGuid(studentId, "Invalid student id");
            var allowed = await CurrentGuardianLinks()
                .AnyAsync(x => x.CanViewAttendance && x.StudentId == studentGuid, cancellationToken);

            if (!allowed)
            {
                throw new UnauthorizedAccessException("You are not allowed to view attendance for this student.");
            }
        }

        public async Task EnsureCurrentGuardianCanViewResultsAsync(string studentEnrollmentId, CancellationToken cancellationToken)
        {
            var enrollmentGuid = ParseGuid(studentEnrollmentId, "Invalid student enrollment id");
            var allowed = await CurrentGuardianLinks()
                .AnyAsync(x =>
                    x.CanViewResults &&
                    x.Student.StudentEnrollments.Any(e => e.Id == enrollmentGuid && !e.IsDeleted),
                    cancellationToken);

            if (!allowed)
            {
                throw new UnauthorizedAccessException("You are not allowed to view results for this student.");
            }
        }

        private IQueryable<GuardianStudent> CurrentGuardianLinks()
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
            {
                throw new Exception("Current user is not available");
            }

            return _context.GuardianStudents
                .AsNoTracking()
                .Where(x => !x.IsDeleted &&
                            x.IsActive &&
                            x.Guardian.UserId == currentUserId &&
                            x.Guardian.IsActive &&
                            !x.Guardian.IsDeleted);
        }

        private async Task ValidateCreateGuardianRequestAsync(Guid studentId, GuardianCreateDto guardianCreateDto, CancellationToken cancellationToken)
        {
            await EnsureStudentExistsAsync(studentId, cancellationToken);

            if (string.IsNullOrWhiteSpace(guardianCreateDto.FullName))
            {
                throw new Exception("Guardian name is required");
            }
            if (string.IsNullOrWhiteSpace(guardianCreateDto.ContactNumber))
            {
                throw new Exception("Guardian contact number is required");
            }
            if (string.IsNullOrWhiteSpace(guardianCreateDto.RelationType))
            {
                throw new Exception("Guardian relation is required");
            }
            if (string.IsNullOrWhiteSpace(guardianCreateDto.UserName) || string.IsNullOrWhiteSpace(guardianCreateDto.Password))
            {
                throw new Exception("Username and password are required");
            }

            var existingUser = await _userManager.FindByNameAsync(guardianCreateDto.UserName.Trim());
            if (existingUser != null)
            {
                throw new Exception("Username already exists");
            }
        }

        private async Task<ApplicationUser> CreateGuardianUserAsync(GuardianCreateDto guardianCreateDto)
        {
            var (firstName, lastName) = SplitFullName(guardianCreateDto.FullName);
            var user = new ApplicationUser
            {
                UserName = guardianCreateDto.UserName.Trim(),
                Email = CleanNullable(guardianCreateDto.Email),
                PhoneNumber = guardianCreateDto.ContactNumber.Trim(),
                FirstName = firstName,
                LastName = lastName,
                ShortName = BuildShortName(firstName, lastName),
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, guardianCreateDto.Password);
            EnsureIdentityResult(result, "Failed to create guardian user");

            var roleResult = await _userManager.AddToRoleAsync(user, GuardianRoleName);
            EnsureIdentityResult(roleResult, "Failed to assign guardian role");

            return user;
        }

        private async Task<GuardianStudent> CreateGuardianStudentLinkAsync(
            Guid guardianId,
            Guid studentId,
            bool isPrimaryGuardian,
            bool canViewFees,
            bool canViewResults,
            bool canViewAttendance,
            bool canPayFees,
            CancellationToken cancellationToken)
        {
            if (isPrimaryGuardian)
            {
                await ClearOtherPrimaryGuardiansAsync(studentId, null, cancellationToken);
            }

            var link = new GuardianStudent
            {
                Id = Guid.NewGuid(),
                GuardianId = guardianId,
                StudentId = studentId,
                IsPrimaryGuardian = isPrimaryGuardian,
                CanViewFees = canViewFees,
                CanViewResults = canViewResults,
                CanViewAttendance = canViewAttendance,
                CanPayFees = canPayFees,
                IsActive = true,
                CreatedBy = GetCurrentUserId(),
                ModifiedBy = GetCurrentUserId()
            };

            await _context.GuardianStudents.AddAsync(link, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return await _context.GuardianStudents
                .AsNoTracking()
                .Include(x => x.Guardian)
                .FirstAsync(x => x.Id == link.Id, cancellationToken);
        }

        private async Task ClearOtherPrimaryGuardiansAsync(Guid studentId, Guid? exceptGuardianStudentId, CancellationToken cancellationToken)
        {
            var otherPrimaryLinks = await _context.GuardianStudents
                .Where(x => x.StudentId == studentId &&
                            x.IsPrimaryGuardian &&
                            !x.IsDeleted &&
                            (!exceptGuardianStudentId.HasValue || x.Id != exceptGuardianStudentId.Value))
                .ToListAsync(cancellationToken);

            foreach (var otherLink in otherPrimaryLinks)
            {
                otherLink.IsPrimaryGuardian = false;
                otherLink.ModifiedBy = GetCurrentUserId();
            }
        }

        private async Task EnsureStudentExistsAsync(Guid studentId, CancellationToken cancellationToken)
        {
            var exists = await _context.Students.AnyAsync(x => x.Id == studentId && !x.IsDeleted, cancellationToken);
            if (!exists)
            {
                throw new Exception("Student not found");
            }
        }

        private async Task EnsureGuardianRoleAsync()
        {
            if (!await _roleManager.RoleExistsAsync(GuardianRoleName))
            {
                var roleResult = await _roleManager.CreateAsync(new ApplicationRole
                {
                    Name = GuardianRoleName,
                    Description = "Guardian with student-scoped access."
                });
                EnsureIdentityResult(roleResult, "Failed to create guardian role");
            }
        }

        private async Task<List<StudentGuardianViewModel>> MapStudentGuardianLinksAsync(List<GuardianStudent> links)
        {
            var result = new List<StudentGuardianViewModel>();
            foreach (var link in links)
            {
                var user = await _userManager.FindByIdAsync(link.Guardian.UserId.ToString());
                result.Add(new StudentGuardianViewModel
                {
                    GuardianStudentId = link.Id,
                    GuardianId = link.GuardianId,
                    UserId = link.Guardian.UserId,
                    FullName = link.Guardian.FullName,
                    ContactNumber = link.Guardian.ContactNumber,
                    Email = link.Guardian.Email,
                    RelationType = link.Guardian.RelationType,
                    UserName = user?.UserName ?? string.Empty,
                    IsGuardianActive = link.Guardian.IsActive && (user?.IsActive ?? false),
                    IsPrimaryGuardian = link.IsPrimaryGuardian,
                    CanViewFees = link.CanViewFees,
                    CanViewResults = link.CanViewResults,
                    CanViewAttendance = link.CanViewAttendance,
                    CanPayFees = link.CanPayFees
                });
            }

            return result;
        }

        private static Guid ParseGuid(string value, string errorMessage)
        {
            if (!Guid.TryParse(value, out var guid))
            {
                throw new Exception(errorMessage);
            }

            return guid;
        }

        private Guid GetCurrentUserId()
        {
            return Guid.TryParse(_userResolver.UserId, out var userId) ? userId : Guid.Empty;
        }

        private static string? CleanNullable(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private static (string FirstName, string LastName) SplitFullName(string fullName)
        {
            var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                return (string.Empty, string.Empty);
            }

            return parts.Length == 1
                ? (parts[0], string.Empty)
                : (parts[0], string.Join(" ", parts.Skip(1)));
        }

        private static string BuildShortName(string firstName, string lastName)
        {
            var initials = string.Concat(new[] { firstName, lastName }
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim()[0]));
            return string.IsNullOrWhiteSpace(initials) ? "G" : initials.ToUpper();
        }

        private static void EnsureIdentityResult(IdentityResult result, string message)
        {
            if (result.Succeeded)
            {
                return;
            }

            var errors = result.Errors.Select(x => x.Description).ToList();
            throw new Exception(errors.Count > 0 ? string.Join(", ", errors) : message);
        }
    }
}
