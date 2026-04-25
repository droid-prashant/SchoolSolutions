namespace Domain
{
    public class TeacherDocument : AuditableEntry
    {
        public Guid TeacherId { get; set; }
        public Teacher Teacher { get; set; }
        public string DocumentType { get; set; }
        public string DocumentTitle { get; set; }
        public string FilePath { get; set; }
        public string OriginalFileName { get; set; }
        public string MimeType { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadedDate { get; set; }
    }
}
