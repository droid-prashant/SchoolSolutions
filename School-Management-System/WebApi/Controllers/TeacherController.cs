using Application.Teachers;
using Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherController : ApiBaseController
    {
        private static readonly string[] AllowedDocumentExtensions = [".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx"];
        private readonly ITeacherService _teacherService;
        private readonly IWebHostEnvironment _environment;

        public TeacherController(ITeacherService teacherService, IWebHostEnvironment environment)
        {
            _teacherService = teacherService;
            _environment = environment;
        }

        [HttpGet("GetTeachers")]
        [HasPermission(PermissionNames.TeacherView, PermissionNames.TeacherManage)]
        public async Task<List<TeacherViewModel>> GetTeachers([FromQuery] string? academicYearId, [FromQuery] bool includeInactive, CancellationToken cancellationToken)
        {
            return await _teacherService.GetTeachersAsync(academicYearId, includeInactive, cancellationToken);
        }

        [HttpGet("GetTeacherById")]
        [HasPermission(PermissionNames.TeacherView, PermissionNames.TeacherManage)]
        public async Task<TeacherViewModel?> GetTeacherById([FromQuery] string teacherId, CancellationToken cancellationToken)
        {
            return await _teacherService.GetTeacherByIdAsync(teacherId, cancellationToken);
        }

        [HttpPost("AddTeacher")]
        [HasPermission(PermissionNames.TeacherManage)]
        public async Task AddTeacher([FromBody] TeacherDto teacherDto, CancellationToken cancellationToken)
        {
            await _teacherService.AddTeacherAsync(teacherDto, cancellationToken);
        }

        [HttpPut("UpdateTeacher")]
        [HasPermission(PermissionNames.TeacherManage)]
        public async Task UpdateTeacher([FromBody] TeacherDto teacherDto, CancellationToken cancellationToken)
        {
            await _teacherService.UpdateTeacherAsync(teacherDto, cancellationToken);
        }

        [HttpPut("UpdateTeacherStatus")]
        [HasPermission(PermissionNames.TeacherManage)]
        public async Task UpdateTeacherStatus([FromQuery] string teacherId, [FromBody] TeacherStatusDto teacherStatusDto, CancellationToken cancellationToken)
        {
            await _teacherService.UpdateTeacherStatusAsync(teacherId, teacherStatusDto, cancellationToken);
        }

        [HttpDelete("DeleteTeacher")]
        [HasPermission(PermissionNames.TeacherManage)]
        public async Task DeleteTeacher([FromQuery] string teacherId, CancellationToken cancellationToken)
        {
            await _teacherService.SoftDeleteTeacherAsync(teacherId, cancellationToken);
        }

        [HttpGet("GetTeacherAssignments")]
        [HasPermission(PermissionNames.TeacherView, PermissionNames.TeacherManage)]
        public async Task<List<TeacherClassSectionViewModel>> GetTeacherAssignments([FromQuery] string teacherId, [FromQuery] string? academicYearId, CancellationToken cancellationToken)
        {
            return await _teacherService.GetTeacherAssignmentsAsync(teacherId, academicYearId, cancellationToken);
        }

        [HttpPost("AddTeacherAssignment")]
        [HasPermission(PermissionNames.TeacherManage)]
        public async Task AddTeacherAssignment([FromQuery] string teacherId, [FromBody] TeacherClassSectionDto assignment, CancellationToken cancellationToken)
        {
            await _teacherService.AddTeacherAssignmentAsync(teacherId, assignment, cancellationToken);
        }

        [HttpPut("UpdateTeacherAssignment")]
        [HasPermission(PermissionNames.TeacherManage)]
        public async Task UpdateTeacherAssignment([FromQuery] string teacherId, [FromBody] TeacherClassSectionDto assignment, CancellationToken cancellationToken)
        {
            await _teacherService.UpdateTeacherAssignmentAsync(teacherId, assignment, cancellationToken);
        }

        [HttpDelete("DeleteTeacherAssignment")]
        [HasPermission(PermissionNames.TeacherManage)]
        public async Task DeleteTeacherAssignment([FromQuery] string assignmentId, CancellationToken cancellationToken)
        {
            await _teacherService.DeleteTeacherAssignmentAsync(assignmentId, cancellationToken);
        }

        [HttpPost("CopyTeacherAssignments")]
        [HasPermission(PermissionNames.TeacherManage)]
        public async Task CopyTeacherAssignments([FromQuery] string teacherId, [FromBody] TeacherAssignmentCopyDto teacherAssignmentCopyDto, CancellationToken cancellationToken)
        {
            await _teacherService.CopyTeacherAssignmentsAsync(teacherId, teacherAssignmentCopyDto, cancellationToken);
        }

        [HttpGet("GetTeacherDashboard")]
        [HasPermission(PermissionNames.TeacherView, PermissionNames.TeacherManage)]
        public async Task<TeacherDashboardViewModel> GetTeacherDashboard([FromQuery] string? academicYearId, CancellationToken cancellationToken)
        {
            return await _teacherService.GetTeacherDashboardAsync(academicYearId, cancellationToken);
        }

        [HttpPost("UploadTeacherDocument")]
        [HasPermission(PermissionNames.TeacherManage)]
        [Consumes("multipart/form-data")]
        public async Task<TeacherDocumentViewModel> UploadTeacherDocument([FromForm] TeacherDocumentUploadRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.TeacherId))
            {
                throw new Exception("Teacher is required");
            }
            if (string.IsNullOrWhiteSpace(request.DocumentType))
            {
                throw new Exception("Document type is required");
            }
            if (string.IsNullOrWhiteSpace(request.DocumentTitle))
            {
                throw new Exception("Document title is required");
            }
            if (request.File == null || request.File.Length == 0)
            {
                throw new Exception("Document file is required");
            }

            if (request.File.Length > 10 * 1024 * 1024)
            {
                throw new Exception("Document size must be 10 MB or less");
            }

            var extension = Path.GetExtension(request.File.FileName).ToLowerInvariant();
            if (!AllowedDocumentExtensions.Contains(extension))
            {
                throw new Exception("Only PDF, image, and Word documents are allowed");
            }

            var teacherId = request.TeacherId.Trim();
            var webRoot = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var relativeFolder = Path.Combine("uploads", "teachers", teacherId);
            var uploadFolder = Path.Combine(webRoot, relativeFolder);
            Directory.CreateDirectory(uploadFolder);

            var storedFileName = $"{Guid.NewGuid()}{extension}";
            var absolutePath = Path.Combine(uploadFolder, storedFileName);
            await using (var stream = System.IO.File.Create(absolutePath))
            {
                await request.File.CopyToAsync(stream, cancellationToken);
            }

            var relativePath = Path.Combine(relativeFolder, storedFileName).Replace("\\", "/");
            try
            {
                return await _teacherService.AddTeacherDocumentAsync(new TeacherDocumentDto
                {
                    TeacherId = teacherId,
                    DocumentType = request.DocumentType.Trim(),
                    DocumentTitle = request.DocumentTitle.Trim(),
                    FilePath = relativePath,
                    OriginalFileName = request.File.FileName,
                    MimeType = request.File.ContentType,
                    FileSize = request.File.Length
                }, cancellationToken);
            }
            catch
            {
                if (System.IO.File.Exists(absolutePath))
                {
                    System.IO.File.Delete(absolutePath);
                }
                throw;
            }
        }

        [HttpDelete("DeleteTeacherDocument")]
        [HasPermission(PermissionNames.TeacherManage)]
        public async Task DeleteTeacherDocument([FromQuery] string documentId, CancellationToken cancellationToken)
        {
            await _teacherService.DeleteTeacherDocumentAsync(documentId, cancellationToken);
        }

        [HttpGet("GetTeacherAccount")]
        [HasPermission(PermissionNames.TeacherManage, PermissionNames.UserManage)]
        public async Task<TeacherAccountViewModel> GetTeacherAccount([FromQuery] string teacherId, CancellationToken cancellationToken)
        {
            return await _teacherService.GetTeacherAccountAsync(teacherId, cancellationToken);
        }

        [HttpPost("CreateTeacherUser")]
        [HasPermission(PermissionNames.TeacherManage, PermissionNames.UserManage)]
        public async Task CreateTeacherUser([FromQuery] string teacherId, [FromBody] TeacherAccountCreateDto teacherAccountCreateDto, CancellationToken cancellationToken)
        {
            await _teacherService.CreateTeacherUserAsync(teacherId, teacherAccountCreateDto, cancellationToken);
        }

        [HttpPut("UpdateTeacherUserStatus")]
        [HasPermission(PermissionNames.TeacherManage, PermissionNames.UserManage)]
        public async Task UpdateTeacherUserStatus([FromQuery] string teacherId, [FromBody] TeacherAccountStatusDto teacherAccountStatusDto, CancellationToken cancellationToken)
        {
            await _teacherService.UpdateTeacherUserStatusAsync(teacherId, teacherAccountStatusDto, cancellationToken);
        }

        [HttpPut("ResetTeacherUserPassword")]
        [HasPermission(PermissionNames.TeacherManage, PermissionNames.UserManage)]
        public async Task ResetTeacherUserPassword([FromQuery] string teacherId, [FromBody] TeacherPasswordResetDto teacherPasswordResetDto, CancellationToken cancellationToken)
        {
            await _teacherService.ResetTeacherUserPasswordAsync(teacherId, teacherPasswordResetDto, cancellationToken);
        }

        [HttpGet("ViewTeacherDocument")]
        [HasPermission(PermissionNames.TeacherView, PermissionNames.TeacherManage)]
        public async Task<IActionResult> ViewTeacherDocument([FromQuery] string documentId, CancellationToken cancellationToken)
        {
            var document = await _teacherService.GetTeacherDocumentAsync(documentId, cancellationToken);
            if (document == null)
            {
                return NotFound("Document not found");
            }

            var absolutePath = ResolveTeacherDocumentPath(document.FilePath);
            if (absolutePath == null)
            {
                return NotFound("Document file not found");
            }

            return new PhysicalFileResult(
                absolutePath,
                string.IsNullOrWhiteSpace(document.MimeType) ? "application/octet-stream" : document.MimeType)
            {
                EnableRangeProcessing = true
            };
        }

        private string? ResolveTeacherDocumentPath(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return null;
            }

            var normalizedPath = relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()).TrimStart(Path.DirectorySeparatorChar);
            var candidateRoots = new[]
            {
                _environment.WebRootPath,
                Path.Combine(_environment.ContentRootPath, "wwwroot"),
                Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
            };

            return candidateRoots
                .Where(root => !string.IsNullOrWhiteSpace(root))
                .Select(root => Path.Combine(root!, normalizedPath))
                .FirstOrDefault(System.IO.File.Exists);
        }
    }

    public class TeacherDocumentUploadRequest
    {
        public string TeacherId { get; set; }
        public string DocumentType { get; set; }
        public string DocumentTitle { get; set; }
        public IFormFile File { get; set; }
    }
}
