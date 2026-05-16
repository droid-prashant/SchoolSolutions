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
    public class NotificationService : INotificationService
    {
        private readonly IApplicationDbContext _context;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly ISignalRNotificationService _signalRNotificationService;
        private readonly UserResolver _userResolver;

        public NotificationService(
            IApplicationDbContext context,
            IPushNotificationService pushNotificationService,
            ISignalRNotificationService signalRNotificationService,
            UserResolver userResolver)
        {
            _context = context;
            _pushNotificationService = pushNotificationService;
            _signalRNotificationService = signalRNotificationService;
            _userResolver = userResolver;
        }

        public async Task RegisterTokenAsync(RegisterNotificationTokenDto request, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var fcmToken = request.FcmToken?.Trim();
            if (string.IsNullOrWhiteSpace(fcmToken))
            {
                throw new ArgumentException("FCM token is required.");
            }

            var now = DateTime.UtcNow;
            var existingToken = await _context.UserNotificationTokens
                .FirstOrDefaultAsync(x => x.UserId == userId && x.FcmToken == fcmToken, cancellationToken);

            if (existingToken == null)
            {
                await _context.UserNotificationTokens.AddAsync(new UserNotificationToken
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    FcmToken = fcmToken,
                    DeviceType = TrimToLength(request.DeviceType, 50),
                    Browser = TrimToLength(request.Browser, 100),
                    Platform = TrimToLength(request.Platform, 100),
                    IsActive = true,
                    LastUsedAt = now,
                    CreatedAt = now,
                    CreatedBy = userId
                }, cancellationToken);
            }
            else
            {
                existingToken.DeviceType = TrimToLength(request.DeviceType, 50);
                existingToken.Browser = TrimToLength(request.Browser, 100);
                existingToken.Platform = TrimToLength(request.Platform, 100);
                existingToken.IsActive = true;
                existingToken.LastUsedAt = now;
                existingToken.UpdatedAt = now;
                existingToken.UpdatedBy = userId;
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeactivateTokenAsync(DeactivateNotificationTokenDto request, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var fcmToken = request.FcmToken?.Trim();
            if (string.IsNullOrWhiteSpace(fcmToken))
            {
                throw new ArgumentException("FCM token is required.");
            }

            var token = await _context.UserNotificationTokens
                .FirstOrDefaultAsync(x => x.UserId == userId && x.FcmToken == fcmToken && x.IsActive, cancellationToken);

            if (token == null)
            {
                return;
            }

            token.IsActive = false;
            token.UpdatedAt = DateTime.UtcNow;
            token.UpdatedBy = userId;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<NotificationViewModel>> GetMyNotificationsAsync(CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            return await _context.UserNotifications
                .AsNoTracking()
                .Where(x => x.UserId == userId && !x.Notification.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new NotificationViewModel
                {
                    Id = x.Id,
                    NotificationId = x.NotificationId,
                    Title = x.Notification.Title,
                    Message = x.Notification.Message,
                    NotificationType = x.Notification.NotificationType,
                    ReferenceId = x.Notification.ReferenceId,
                    StudentId = x.Notification.StudentId,
                    StudentName = x.Notification.Student == null ? null : (x.Notification.Student.FirstName + " " + x.Notification.Student.LastName).Trim(),
                    NoticeId = x.Notification.NotificationType == NotificationTypes.Notice ? x.Notification.ReferenceId : null,
                    IsRead = x.IsRead,
                    ReadAt = x.ReadAt,
                    CreatedAt = x.CreatedAt,
                    NotificationDateTimeNp = x.Notification.NotificationDateTimeNp,
                    DeliveryStatus = x.DeliveryStatus
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<NoticeLetterViewModel> GetMyNoticeLetterAsync(string noticeId, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (!Guid.TryParse(noticeId, out var noticeGuid))
            {
                throw new ArgumentException("Invalid notice id.");
            }

            var deliveredNotice = await _context.UserNotifications
                .AsNoTracking()
                .Include(x => x.Notification)
                .Where(x => x.UserId == userId &&
                            !x.Notification.IsDeleted &&
                            x.Notification.NotificationType == NotificationTypes.Notice &&
                            x.Notification.ReferenceId == noticeGuid)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (deliveredNotice == null)
            {
                throw new KeyNotFoundException("Notice notification not found.");
            }

            var notice = await _context.Notices
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == noticeGuid && !x.IsDeleted && x.IsPublished, cancellationToken);

            if (notice == null)
            {
                throw new KeyNotFoundException("Notice not found.");
            }

            var school = await _context.Schools
                .AsNoTracking()
                .OrderBy(x => x.CreatedDate)
                .FirstOrDefaultAsync(cancellationToken);

            return new NoticeLetterViewModel
            {
                NoticeId = notice.Id,
                SchoolName = string.IsNullOrWhiteSpace(school?.Name) ? "Om Pushpanjali English School" : school.Name,
                SchoolAddress = BuildSchoolAddress(school),
                SchoolPhoneNumber = school?.PhoneNumber ?? string.Empty,
                SchoolEmail = school?.Email ?? string.Empty,
                SchoolWebsite = school?.Website ?? string.Empty,
                NoticeNumber = $"NOTICE-{notice.CreatedAt:yyyyMMdd}-{notice.Id.ToString("N")[..6].ToUpperInvariant()}",
                NoticeDate = notice.NoticeDate,
                NoticeDateNp = notice.NoticeDateNp,
                PublishedDateTimeNp = deliveredNotice.Notification.NotificationDateTimeNp,
                Subject = notice.Title,
                Body = notice.Description,
                TargetAudience = notice.TargetAudience,
                IssuedBy = "School Administration"
            };
        }

        public async Task<int> GetUnreadCountAsync(CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            return await _context.UserNotifications
                .CountAsync(x => x.UserId == userId && !x.IsRead && !x.Notification.IsDeleted, cancellationToken);
        }

        public async Task MarkAsReadAsync(string userNotificationId, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (!Guid.TryParse(userNotificationId, out var notificationGuid))
            {
                throw new ArgumentException("Invalid notification id.");
            }

            var userNotification = await _context.UserNotifications
                .FirstOrDefaultAsync(x => x.Id == notificationGuid && x.UserId == userId, cancellationToken);

            if (userNotification == null)
            {
                throw new KeyNotFoundException("Notification not found.");
            }

            MarkRead(userNotification);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task MarkAllAsReadAsync(CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var notifications = await _context.UserNotifications
                .Where(x => x.UserId == userId && !x.IsRead)
                .ToListAsync(cancellationToken);

            foreach (var notification in notifications)
            {
                MarkRead(notification);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateAttendanceNotificationsAsync(IReadOnlyCollection<Guid> attendanceIds, CancellationToken cancellationToken)
        {
            if (attendanceIds.Count == 0)
            {
                return;
            }

            var attendanceRows = await _context.StudentAttendances
                .Include(x => x.Student)
                .Where(x => attendanceIds.Contains(x.Id) && !x.IsDeleted)
                .ToListAsync(cancellationToken);

            var now = DateTime.UtcNow;
            var createdBy = TryGetCurrentUserId();
            var studentIds = attendanceRows.Select(x => x.StudentId).Distinct().ToList();
            var guardianLinks = await _context.GuardianStudents
                .Include(x => x.Guardian)
                .Where(x => studentIds.Contains(x.StudentId) &&
                            !x.IsDeleted &&
                            x.IsActive &&
                            x.CanViewAttendance &&
                            x.Guardian.IsActive &&
                            !x.Guardian.IsDeleted)
                .ToListAsync(cancellationToken);

            var dispatchItems = new List<NotificationDispatchDto>();

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            foreach (var attendance in attendanceRows)
            {
                var notificationDateTimeNp = NepaliDateTimeHelper.FromBsDateAndUtcTime(attendance.AttendanceDateNp, now);
                var notification = await _context.Notifications
                    .FirstOrDefaultAsync(x => x.NotificationType == NotificationTypes.Attendance &&
                                              x.ReferenceId == attendance.Id &&
                                              x.StudentId == attendance.StudentId &&
                                              !x.IsDeleted, cancellationToken);

                if (notification == null)
                {
                    notification = new Notification
                    {
                        Id = Guid.NewGuid(),
                        Title = "Attendance Marked",
                        Message = $"Your child {BuildStudentName(attendance.Student)} has been marked {attendance.Status} on {notificationDateTimeNp}.",
                        NotificationType = NotificationTypes.Attendance,
                        ReferenceId = attendance.Id,
                        StudentId = attendance.StudentId,
                        CreatedAt = now,
                        NotificationDateTimeNp = notificationDateTimeNp,
                        CreatedBy = createdBy,
                        IsDeleted = false
                    };
                    await _context.Notifications.AddAsync(notification, cancellationToken);
                }

                var guardians = guardianLinks
                    .Where(x => x.StudentId == attendance.StudentId)
                    .Select(x => x.Guardian.UserId)
                    .Distinct()
                    .ToList();

                foreach (var guardianUserId in guardians)
                {
                    var existingUserNotification = await _context.UserNotifications
                        .AnyAsync(x => x.NotificationId == notification.Id && x.UserId == guardianUserId, cancellationToken);

                    if (existingUserNotification)
                    {
                        continue;
                    }

                    var userNotification = CreateUserNotification(notification.Id, guardianUserId, now);
                    await _context.UserNotifications.AddAsync(userNotification, cancellationToken);
                    dispatchItems.Add(ToDispatchDto(userNotification, notification));
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            await DispatchAsync(dispatchItems, cancellationToken);
        }

        public async Task<List<NotificationDispatchDto>> CreateNoticeNotificationAsync(Guid noticeId, CancellationToken cancellationToken)
        {
            var notice = await _context.Notices
                .FirstOrDefaultAsync(x => x.Id == noticeId && !x.IsDeleted, cancellationToken);

            if (notice == null)
            {
                throw new KeyNotFoundException("Notice not found.");
            }

            var recipients = await GetNoticeGuardianUserIdsAsync(notice, cancellationToken);
            if (recipients.Count == 0)
            {
                return [];
            }

            var now = DateTime.UtcNow;
            var notificationDateTimeNp = NepaliDateTimeHelper.FromUtc(now);
            var createdBy = TryGetCurrentUserId();
            var dispatchItems = new List<NotificationDispatchDto>();

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(x => x.NotificationType == NotificationTypes.Notice &&
                                          x.ReferenceId == notice.Id &&
                                          !x.IsDeleted, cancellationToken);

            if (notification == null)
            {
                notification = new Notification
                {
                    Id = Guid.NewGuid(),
                    Title = "New School Notice",
                    Message = notice.Title,
                    NotificationType = NotificationTypes.Notice,
                    ReferenceId = notice.Id,
                    CreatedAt = now,
                    NotificationDateTimeNp = notificationDateTimeNp,
                    CreatedBy = createdBy,
                    IsDeleted = false
                };
                await _context.Notifications.AddAsync(notification, cancellationToken);
            }

            foreach (var userId in recipients)
            {
                var existingUserNotification = await _context.UserNotifications
                    .AnyAsync(x => x.NotificationId == notification.Id && x.UserId == userId, cancellationToken);

                if (existingUserNotification)
                {
                    continue;
                }

                var userNotification = CreateUserNotification(notification.Id, userId, now);
                await _context.UserNotifications.AddAsync(userNotification, cancellationToken);
                dispatchItems.Add(ToDispatchDto(userNotification, notification));
            }

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return dispatchItems;
        }

        public async Task DispatchAsync(IReadOnlyCollection<NotificationDispatchDto> notifications, CancellationToken cancellationToken)
        {
            foreach (var notification in notifications)
            {
                var userNotification = await _context.UserNotifications
                    .FirstOrDefaultAsync(x => x.Id == notification.UserNotificationId, cancellationToken);

                if (userNotification == null)
                {
                    continue;
                }

                string? errorMessage = null;
                var pushResult = await _pushNotificationService.SendToUserAsync(notification, cancellationToken);
                if (pushResult.Succeeded)
                {
                    userNotification.IsPushSent = true;
                    userNotification.PushSentAt = DateTime.UtcNow;
                }
                else
                {
                    errorMessage = pushResult.ErrorMessage;
                }

                var signalRSent = await _signalRNotificationService.SendToUserAsync(notification, cancellationToken);
                if (signalRSent)
                {
                    userNotification.IsSignalRSent = true;
                    userNotification.SignalRSentAt = DateTime.UtcNow;
                }

                userNotification.DeliveryStatus = userNotification.IsPushSent || userNotification.IsSignalRSent
                    ? NotificationDeliveryStatuses.Sent
                    : NotificationDeliveryStatuses.Failed;
                userNotification.ErrorMessage = userNotification.DeliveryStatus == NotificationDeliveryStatuses.Failed
                    ? errorMessage ?? "No active delivery channel was available."
                    : null;

                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        private async Task<List<Guid>> GetNoticeGuardianUserIdsAsync(Notice notice, CancellationToken cancellationToken)
        {
            var query = _context.GuardianStudents
                .AsNoTracking()
                .Include(x => x.Guardian)
                .Include(x => x.Student)
                    .ThenInclude(x => x.StudentEnrollments)
                        .ThenInclude(x => x.ClassSection)
                .Where(x => !x.IsDeleted &&
                            x.IsActive &&
                            x.Guardian.IsActive &&
                            !x.Guardian.IsDeleted);

            if (notice.TargetAudience == NoticeTargetAudiences.StudentWise)
            {
                var studentIds = await _context.NoticeStudents
                    .AsNoTracking()
                    .Where(x => x.NoticeId == notice.Id)
                    .Select(x => x.StudentId)
                    .Distinct()
                    .ToListAsync(cancellationToken);

                if (studentIds.Count == 0)
                {
                    throw new InvalidOperationException("Students are required for student-wise notice.");
                }

                query = query.Where(x => studentIds.Contains(x.StudentId));
            }
            else if (notice.TargetAudience == NoticeTargetAudiences.ClassWise)
            {
                if (!notice.ClassId.HasValue)
                {
                    throw new InvalidOperationException("Class is required for class-wise notice.");
                }

                query = query.Where(x => x.Student.StudentEnrollments.Any(e => e.IsActive && !e.IsDeleted && e.ClassSection.ClassId == notice.ClassId.Value));
            }
            else if (notice.TargetAudience == NoticeTargetAudiences.SectionWise)
            {
                if (!notice.ClassId.HasValue || !notice.SectionId.HasValue)
                {
                    throw new InvalidOperationException("Class and section are required for section-wise notice.");
                }

                query = query.Where(x => x.Student.StudentEnrollments.Any(e => e.IsActive &&
                                                                               !e.IsDeleted &&
                                                                               e.ClassSection.SectionId == notice.SectionId.Value &&
                                                                               e.ClassSection.ClassId == notice.ClassId.Value));
            }
            else if (notice.TargetAudience != NoticeTargetAudiences.AllGuardians)
            {
                throw new InvalidOperationException("Invalid notice target audience.");
            }

            return await query
                .Select(x => x.Guardian.UserId)
                .Distinct()
                .ToListAsync(cancellationToken);
        }

        private Guid GetCurrentUserId()
        {
            if (!Guid.TryParse(_userResolver.UserId, out var userId))
            {
                throw new InvalidOperationException("Current user is not available.");
            }

            return userId;
        }

        private Guid? TryGetCurrentUserId()
        {
            return Guid.TryParse(_userResolver.UserId, out var userId) ? userId : null;
        }

        private static UserNotification CreateUserNotification(Guid notificationId, Guid userId, DateTime createdAt)
        {
            return new UserNotification
            {
                Id = Guid.NewGuid(),
                NotificationId = notificationId,
                UserId = userId,
                IsRead = false,
                DeliveryStatus = NotificationDeliveryStatuses.Pending,
                CreatedAt = createdAt
            };
        }

        private static NotificationDispatchDto ToDispatchDto(UserNotification userNotification, Notification notification)
        {
            return new NotificationDispatchDto
            {
                UserNotificationId = userNotification.Id,
                UserId = userNotification.UserId,
                NotificationId = notification.Id,
                Title = notification.Title,
                Message = notification.Message,
                NotificationType = notification.NotificationType,
                NotificationDateTimeNp = notification.NotificationDateTimeNp,
                ReferenceId = notification.ReferenceId,
                StudentId = notification.StudentId
            };
        }

        private static void MarkRead(UserNotification userNotification)
        {
            if (userNotification.IsRead)
            {
                return;
            }

            userNotification.IsRead = true;
            userNotification.ReadAt = DateTime.UtcNow;
        }

        private static string BuildStudentName(Student student)
        {
            return $"{student.FirstName} {student.LastName}".Trim();
        }

        private static string BuildSchoolAddress(School? school)
        {
            if (school == null)
            {
                return string.Empty;
            }

            var addressParts = new[]
            {
                school.Address,
                school.City,
                school.District,
                school.Province
            }
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim());

            var address = string.Join(", ", addressParts);
            return school.WardNo > 0
                ? $"{address} - {school.WardNo}".Trim(' ', '-')
                : address;
        }

        private static string? TrimToLength(string? value, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var trimmed = value.Trim();
            return trimmed.Length <= maxLength ? trimmed : trimmed[..maxLength];
        }
    }
}
