namespace Domain
{
    public class UserNotification
    {
        public Guid Id { get; set; }
        public Guid NotificationId { get; set; }
        public Notification Notification { get; set; } = null!;
        public Guid UserId { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public bool IsPushSent { get; set; }
        public DateTime? PushSentAt { get; set; }
        public bool IsSignalRSent { get; set; }
        public DateTime? SignalRSentAt { get; set; }
        public string DeliveryStatus { get; set; } = "Pending";
        public string? ErrorMessage { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
