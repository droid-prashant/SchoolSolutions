using System;

namespace Application.Students.ViewModels
{
    public class PromotionCandidateViewModel
    {
        public Guid StudentEnrollmentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public int? RollNumber { get; set; }
        public string CurrentClassName { get; set; } = string.Empty;
        public string CurrentSectionName { get; set; } = string.Empty;
        public int ExamType { get; set; }
        public decimal? GPA { get; set; }
        public string ResultStatus { get; set; } = string.Empty;
        public bool IsPromotable { get; set; }
        public bool IsAlreadyPromoted { get; set; }
        public string TargetClassName { get; set; } = string.Empty;
        public string TargetSectionName { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
    }
}
