using System.Collections.Generic;

namespace Application.Students.Dtos
{
    public class PromoteStudentsDto
    {
        public string ClassSectionId { get; set; } = string.Empty;
        public int ExamType { get; set; }
        public string TargetAcademicYearId { get; set; } = string.Empty;
        public string? TargetClassSectionId { get; set; }
        public bool PromoteAllEligible { get; set; }
        public List<string> StudentEnrollmentIds { get; set; } = new();
    }
}
