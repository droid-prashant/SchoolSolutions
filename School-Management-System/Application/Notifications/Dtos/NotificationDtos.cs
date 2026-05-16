namespace Application.Notifications.Dtos
{
    public class RegisterNotificationTokenDto
    {
        public string FcmToken { get; set; } = string.Empty;
        public string? DeviceType { get; set; }
        public string? Browser { get; set; }
        public string? Platform { get; set; }
    }

    public class DeactivateNotificationTokenDto
    {
        public string FcmToken { get; set; } = string.Empty;
    }

    public class CreateNoticeDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime NoticeDate { get; set; }
        public string NoticeDateNp { get; set; } = string.Empty;
        public string TargetAudience { get; set; } = "AllGuardians";
        public string? ClassId { get; set; }
        public string? SectionId { get; set; }
        public List<string> StudentIds { get; set; } = [];
    }

    public class UpdateNoticeDto : CreateNoticeDto
    {
    }

    public class NotificationDispatchDto
    {
        public Guid UserNotificationId { get; set; }
        public Guid UserId { get; set; }
        public Guid NotificationId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string NotificationType { get; set; } = string.Empty;
        public string NotificationDateTimeNp { get; set; } = string.Empty;
        public Guid? ReferenceId { get; set; }
        public Guid? StudentId { get; set; }
    }

    public class PushNotificationResultDto
    {
        public bool Succeeded { get; set; }
        public string? ErrorMessage { get; set; }
        public IReadOnlyList<string> InvalidTokens { get; set; } = [];
    }
}
