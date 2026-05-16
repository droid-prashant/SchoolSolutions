using Application.Common.Interfaces;
using Application.Notifications.Dtos;
using Application.Notifications.Interfaces;
using Application.Notifications.ViewModels;
using Domain;
using Domain.Constants;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Notifications
{
    public class NoticeService : INoticeService
    {
        private readonly IApplicationDbContext _context;
        private readonly INotificationService _notificationService;
        private readonly UserResolver _userResolver;

        public NoticeService(
            IApplicationDbContext context,
            INotificationService notificationService,
            UserResolver userResolver)
        {
            _context = context;
            _notificationService = notificationService;
            _userResolver = userResolver;
        }

        public async Task<NoticeViewModel> CreateAsync(CreateNoticeDto request, CancellationToken cancellationToken)
        {
            ValidateNotice(request);
            var now = DateTime.UtcNow;
            var currentUserId = TryGetCurrentUserId();

            var notice = new Notice
            {
                Id = Guid.NewGuid(),
                Title = request.Title.Trim(),
                Description = request.Description.Trim(),
                NoticeDate = EnsureUtc(request.NoticeDate),
                NoticeDateNp = request.NoticeDateNp.Trim(),
                TargetAudience = request.TargetAudience.Trim(),
                ClassId = ParseOptionalGuid(request.ClassId, "Invalid class id."),
                SectionId = ParseOptionalGuid(request.SectionId, "Invalid section id."),
                CreatedAt = now,
                CreatedBy = currentUserId,
                IsPublished = false,
                IsDeleted = false
            };

            await _context.Notices.AddAsync(notice, cancellationToken);
            AddNoticeStudents(notice, request.StudentIds, now);
            await _context.SaveChangesAsync(cancellationToken);
            return await GetByIdAsync(notice.Id.ToString(), cancellationToken);
        }

        public async Task<NoticeViewModel> UpdateAsync(string noticeId, UpdateNoticeDto request, CancellationToken cancellationToken)
        {
            ValidateNotice(request);
            var noticeGuid = ParseGuid(noticeId, "Invalid notice id.");
            var notice = await _context.Notices
                .FirstOrDefaultAsync(x => x.Id == noticeGuid && !x.IsDeleted, cancellationToken);

            if (notice == null)
            {
                throw new KeyNotFoundException("Notice not found.");
            }

            if (notice.IsPublished)
            {
                throw new InvalidOperationException("Published notices cannot be edited.");
            }

            notice.Title = request.Title.Trim();
            notice.Description = request.Description.Trim();
            notice.NoticeDate = EnsureUtc(request.NoticeDate);
            notice.NoticeDateNp = request.NoticeDateNp.Trim();
            notice.TargetAudience = request.TargetAudience.Trim();
            notice.ClassId = ParseOptionalGuid(request.ClassId, "Invalid class id.");
            notice.SectionId = ParseOptionalGuid(request.SectionId, "Invalid section id.");
            notice.UpdatedAt = DateTime.UtcNow;
            notice.UpdatedBy = TryGetCurrentUserId();

            await ReplaceNoticeStudentsAsync(notice.Id, notice.TargetAudience, request.StudentIds, notice.UpdatedAt.Value, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return await GetByIdAsync(notice.Id.ToString(), cancellationToken);
        }

        public async Task PublishAsync(string noticeId, CancellationToken cancellationToken)
        {
            var noticeGuid = ParseGuid(noticeId, "Invalid notice id.");
            var notice = await _context.Notices
                .FirstOrDefaultAsync(x => x.Id == noticeGuid && !x.IsDeleted, cancellationToken);

            if (notice == null)
            {
                throw new KeyNotFoundException("Notice not found.");
            }

            if (notice.IsPublished)
            {
                return;
            }

            ValidateNoticeTarget(notice.TargetAudience, notice.ClassId, notice.SectionId);

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            notice.IsPublished = true;
            notice.PublishedAt = DateTime.UtcNow;
            notice.UpdatedAt = DateTime.UtcNow;
            notice.UpdatedBy = TryGetCurrentUserId();
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            var dispatchItems = await _notificationService.CreateNoticeNotificationAsync(notice.Id, cancellationToken);
            await _notificationService.DispatchAsync(dispatchItems, cancellationToken);
        }

        public async Task<List<NoticeViewModel>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Notices
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.NoticeDate)
                .ThenByDescending(x => x.CreatedAt)
                .Select(x => new NoticeViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    NoticeDate = x.NoticeDate,
                    NoticeDateNp = x.NoticeDateNp,
                    TargetAudience = x.TargetAudience,
                    ClassId = x.ClassId,
                    ClassName = x.Class == null ? null : x.Class.Name,
                    SectionId = x.SectionId,
                    SectionName = x.Section == null ? null : x.Section.Name,
                    StudentIds = x.NoticeStudents.Select(y => y.StudentId).ToList(),
                    StudentNames = x.NoticeStudents
                        .Select(y => (y.Student.FirstName + " " + y.Student.LastName).Trim())
                        .ToList(),
                    IsPublished = x.IsPublished,
                    PublishedAt = x.PublishedAt,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<NoticeViewModel> GetByIdAsync(string noticeId, CancellationToken cancellationToken)
        {
            var noticeGuid = ParseGuid(noticeId, "Invalid notice id.");
            var notice = await _context.Notices
                .AsNoTracking()
                .Where(x => x.Id == noticeGuid && !x.IsDeleted)
                .Select(x => new NoticeViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    NoticeDate = x.NoticeDate,
                    NoticeDateNp = x.NoticeDateNp,
                    TargetAudience = x.TargetAudience,
                    ClassId = x.ClassId,
                    ClassName = x.Class == null ? null : x.Class.Name,
                    SectionId = x.SectionId,
                    SectionName = x.Section == null ? null : x.Section.Name,
                    StudentIds = x.NoticeStudents.Select(y => y.StudentId).ToList(),
                    StudentNames = x.NoticeStudents
                        .Select(y => (y.Student.FirstName + " " + y.Student.LastName).Trim())
                        .ToList(),
                    IsPublished = x.IsPublished,
                    PublishedAt = x.PublishedAt,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                })
                .FirstOrDefaultAsync(cancellationToken);

            return notice ?? throw new KeyNotFoundException("Notice not found.");
        }

        private static void ValidateNoticeTarget(string targetAudience, Guid? classId, Guid? sectionId)
        {
            if (targetAudience == NoticeTargetAudiences.ClassWise && !classId.HasValue)
            {
                throw new ArgumentException("Class is required for class-wise notice.");
            }

            if (targetAudience == NoticeTargetAudiences.SectionWise && !sectionId.HasValue)
            {
                throw new ArgumentException("Class and section are required for section-wise notice.");
            }

            if (targetAudience == NoticeTargetAudiences.SectionWise && !classId.HasValue)
            {
                throw new ArgumentException("Class and section are required for section-wise notice.");
            }

            if (targetAudience != NoticeTargetAudiences.AllGuardians &&
                targetAudience != NoticeTargetAudiences.ClassWise &&
                targetAudience != NoticeTargetAudiences.SectionWise &&
                targetAudience != NoticeTargetAudiences.StudentWise)
            {
                throw new ArgumentException("Invalid notice target audience.");
            }
        }

        private static void ValidateNotice(CreateNoticeDto request)
        {
            if (request == null)
            {
                throw new ArgumentException("Notice request is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                throw new ArgumentException("Notice title is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Description))
            {
                throw new ArgumentException("Notice description is required.");
            }

            if (string.IsNullOrWhiteSpace(request.NoticeDateNp))
            {
                throw new ArgumentException("Nepali notice date is required.");
            }

            if (string.IsNullOrWhiteSpace(request.TargetAudience))
            {
                throw new ArgumentException("Notice target audience is required.");
            }

            var targetAudience = request.TargetAudience.Trim();
            var classId = ParseOptionalGuid(request.ClassId, "Invalid class id.");
            var sectionId = ParseOptionalGuid(request.SectionId, "Invalid section id.");

            ValidateNoticeTarget(targetAudience, classId, sectionId);

            if (targetAudience == NoticeTargetAudiences.StudentWise && GetDistinctStudentIds(request.StudentIds).Count == 0)
            {
                throw new ArgumentException("At least one student is required for student-wise notice.");
            }
        }

        private static void AddNoticeStudents(Notice notice, IEnumerable<string> studentIds, DateTime createdAt)
        {
            if (notice.TargetAudience != NoticeTargetAudiences.StudentWise)
            {
                return;
            }

            foreach (var studentId in GetDistinctStudentIds(studentIds))
            {
                notice.NoticeStudents.Add(new NoticeStudent
                {
                    Id = Guid.NewGuid(),
                    NoticeId = notice.Id,
                    StudentId = studentId,
                    CreatedAt = createdAt
                });
            }
        }

        private async Task ReplaceNoticeStudentsAsync(Guid noticeId, string targetAudience, IEnumerable<string> studentIds, DateTime createdAt, CancellationToken cancellationToken)
        {
            var existing = await _context.NoticeStudents
                .Where(x => x.NoticeId == noticeId)
                .ToListAsync(cancellationToken);

            _context.NoticeStudents.RemoveRange(existing);
            if (targetAudience != NoticeTargetAudiences.StudentWise)
            {
                return;
            }

            foreach (var studentId in GetDistinctStudentIds(studentIds))
            {
                await _context.NoticeStudents.AddAsync(new NoticeStudent
                {
                    Id = Guid.NewGuid(),
                    NoticeId = noticeId,
                    StudentId = studentId,
                    CreatedAt = createdAt
                }, cancellationToken);
            }
        }

        private static List<Guid> GetDistinctStudentIds(IEnumerable<string>? studentIds)
        {
            return (studentIds ?? [])
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => ParseGuid(x, "Invalid student id."))
                .Distinct()
                .ToList();
        }

        private Guid? TryGetCurrentUserId()
        {
            return Guid.TryParse(_userResolver.UserId, out var userId) ? userId : null;
        }

        private static Guid ParseGuid(string value, string message)
        {
            if (!Guid.TryParse(value, out var parsed))
            {
                throw new ArgumentException(message);
            }

            return parsed;
        }

        private static Guid? ParseOptionalGuid(string? value, string message)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            if (!Guid.TryParse(value, out var parsed))
            {
                throw new ArgumentException(message);
            }

            return parsed;
        }

        private static DateTime EnsureUtc(DateTime value)
        {
            if (value.Kind == DateTimeKind.Utc)
            {
                return value;
            }

            return DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }
    }
}
