namespace Domain
{
    public class Notice
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime NoticeDate { get; set; }
        public string NoticeDateNp { get; set; } = string.Empty;
        public string TargetAudience { get; set; } = "AllGuardians";
        public Guid? ClassId { get; set; }
        public ClassRoom? Class { get; set; }
        public Guid? SectionId { get; set; }
        public Section? Section { get; set; }
        public ICollection<NoticeStudent> NoticeStudents { get; set; } = new List<NoticeStudent>();
        public bool IsPublished { get; set; }
        public DateTime? PublishedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
