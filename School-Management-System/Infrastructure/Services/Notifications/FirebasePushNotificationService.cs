using Application.Common.Interfaces;
using Application.Notifications.Dtos;
using Application.Notifications.Interfaces;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Notifications
{
    public class FirebasePushNotificationService : IPushNotificationService
    {
        private readonly IApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<FirebasePushNotificationService> _logger;
        private static readonly object FirebaseLock = new();

        public FirebasePushNotificationService(
            IApplicationDbContext context,
            IConfiguration configuration,
            ILogger<FirebasePushNotificationService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<PushNotificationResultDto> SendToUserAsync(NotificationDispatchDto notification, CancellationToken cancellationToken)
        {
            if (!EnsureFirebaseConfigured())
            {
                return new PushNotificationResultDto
                {
                    Succeeded = false,
                    ErrorMessage = "Firebase is not configured."
                };
            }

            var tokens = await _context.UserNotificationTokens
                .Where(x => x.UserId == notification.UserId && x.IsActive)
                .ToListAsync(cancellationToken);

            if (tokens.Count == 0)
            {
                return new PushNotificationResultDto
                {
                    Succeeded = false,
                    ErrorMessage = "No active FCM token found for user."
                };
            }

            var invalidTokens = new List<string>();
            var sentCount = 0;
            var errors = new List<string>();

            foreach (var token in tokens)
            {
                try
                {
                    await FirebaseMessaging.DefaultInstance.SendAsync(new Message
                    {
                        Token = token.FcmToken,
                        Notification = new FirebaseAdmin.Messaging.Notification
                        {
                            Title = notification.Title,
                            Body = string.IsNullOrWhiteSpace(notification.NotificationDateTimeNp)
                                ? notification.Message
                                : $"{notification.Message} ({notification.NotificationDateTimeNp})"
                        },
                        Data = new Dictionary<string, string>
                        {
                            ["userNotificationId"] = notification.UserNotificationId.ToString(),
                            ["notificationId"] = notification.NotificationId.ToString(),
                            ["notificationType"] = notification.NotificationType,
                            ["notificationDateTimeNp"] = notification.NotificationDateTimeNp,
                            ["referenceId"] = notification.ReferenceId?.ToString() ?? string.Empty,
                            ["studentId"] = notification.StudentId?.ToString() ?? string.Empty
                        }
                    }, cancellationToken);

                    sentCount++;
                    token.LastUsedAt = DateTime.UtcNow;
                }
                catch (FirebaseMessagingException exception) when (IsInvalidTokenError(exception))
                {
                    token.IsActive = false;
                    token.UpdatedAt = DateTime.UtcNow;
                    invalidTokens.Add(token.FcmToken);
                    _logger.LogWarning(exception, "Invalid Firebase token detected for user {UserId}", notification.UserId);
                }
                catch (Exception exception)
                {
                    errors.Add(exception.Message);
                    _logger.LogWarning(exception, "Firebase push failed for user {UserId}", notification.UserId);
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new PushNotificationResultDto
            {
                Succeeded = sentCount > 0,
                ErrorMessage = sentCount > 0 ? null : string.Join("; ", errors.DefaultIfEmpty("Firebase push failed.")),
                InvalidTokens = invalidTokens
            };
        }

        private bool EnsureFirebaseConfigured()
        {
            if (FirebaseApp.DefaultInstance != null)
            {
                return true;
            }

            lock (FirebaseLock)
            {
                if (FirebaseApp.DefaultInstance != null)
                {
                    return true;
                }

                var serviceAccountJson = _configuration["Firebase:ServiceAccountJson"]
                    ?? Environment.GetEnvironmentVariable("FIREBASE_SERVICE_ACCOUNT_JSON");
                var serviceAccountPath = _configuration["Firebase:ServiceAccountPath"]
                    ?? Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");

                if (!string.IsNullOrWhiteSpace(serviceAccountJson))
                {
                    FirebaseApp.Create(new AppOptions
                    {
                        Credential = GoogleCredential.FromJson(serviceAccountJson)
                    });
                    return true;
                }

                if (!string.IsNullOrWhiteSpace(serviceAccountPath))
                {
                    FirebaseApp.Create(new AppOptions
                    {
                        Credential = GoogleCredential.FromFile(serviceAccountPath)
                    });
                    return true;
                }
            }

            return false;
        }

        private static bool IsInvalidTokenError(FirebaseMessagingException exception)
        {
            var code = exception.MessagingErrorCode?.ToString() ?? string.Empty;
            return code.Equals("Unregistered", StringComparison.OrdinalIgnoreCase) ||
                   code.Equals("InvalidArgument", StringComparison.OrdinalIgnoreCase) ||
                   code.Equals("SenderIdMismatch", StringComparison.OrdinalIgnoreCase);
        }
    }
}
