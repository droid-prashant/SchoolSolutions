namespace Domain
{
    public class Notification
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string NotificationType { get; set; } = string.Empty;
        public Guid? ReferenceId { get; set; }
        public Guid? StudentId { get; set; }
        public Student? Student { get; set; }
        public DateTime CreatedAt { get; set; }
        public string NotificationDateTimeNp { get; set; } = string.Empty;
        public Guid? CreatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
    }
}
