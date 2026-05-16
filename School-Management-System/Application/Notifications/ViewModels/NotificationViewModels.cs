namespace Application.Notifications.ViewModels
{
    public class NotificationViewModel
    {
        public Guid Id { get; set; }
        public Guid NotificationId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string NotificationType { get; set; } = string.Empty;
        public Guid? ReferenceId { get; set; }
        public Guid? StudentId { get; set; }
        public string? StudentName { get; set; }
        public Guid? NoticeId { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string NotificationDateTimeNp { get; set; } = string.Empty;
        public string DeliveryStatus { get; set; } = string.Empty;
    }

    public class NoticeViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime NoticeDate { get; set; }
        public string NoticeDateNp { get; set; } = string.Empty;
        public string TargetAudience { get; set; } = string.Empty;
        public Guid? ClassId { get; set; }
        public string? ClassName { get; set; }
        public Guid? SectionId { get; set; }
        public string? SectionName { get; set; }
        public List<Guid> StudentIds { get; set; } = [];
        public List<string> StudentNames { get; set; } = [];
        public bool IsPublished { get; set; }
        public DateTime? PublishedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class NoticeLetterViewModel
    {
        public Guid NoticeId { get; set; }
        public string SchoolName { get; set; } = string.Empty;
        public string SchoolAddress { get; set; } = string.Empty;
        public string SchoolPhoneNumber { get; set; } = string.Empty;
        public string SchoolEmail { get; set; } = string.Empty;
        public string SchoolWebsite { get; set; } = string.Empty;
        public string NoticeNumber { get; set; } = string.Empty;
        public DateTime NoticeDate { get; set; }
        public string NoticeDateNp { get; set; } = string.Empty;
        public string PublishedDateTimeNp { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string TargetAudience { get; set; } = string.Empty;
        public string IssuedBy { get; set; } = string.Empty;
    }
}
