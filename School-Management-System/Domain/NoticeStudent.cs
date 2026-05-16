namespace Domain
{
    public class NoticeStudent
    {
        public Guid Id { get; set; }
        public Guid NoticeId { get; set; }
        public Notice Notice { get; set; } = null!;
        public Guid StudentId { get; set; }
        public Student Student { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
