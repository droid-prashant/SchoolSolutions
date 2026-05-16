namespace Domain
{
    public class UserNotificationToken
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string FcmToken { get; set; } = string.Empty;
        public string? DeviceType { get; set; }
        public string? Browser { get; set; }
        public string? Platform { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? LastUsedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
