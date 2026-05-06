using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain
{
    public class StudentEnrollment:AuditableEntry
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Student Student { get; set; }
        public int? RollNumber { get; set; }
        public Guid ClassSectionId { get; set; }
        public ClassSection ClassSection { get; set; }
        public Guid AcademicYearId { get; set; }
        public AcademicYear AcademicYear { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string? RegistrationNumber { get; set; }
        public string? SymbolNumber { get; set; }
        public bool IsPromoted { get; set; }
        public StudentEnrollmentStatus EnrollmentStatus { get; set; }
        public DateTime? StatusDate { get; set; }
        public string? StatusRemarks { get; set; }
        public ICollection<StudentAttendance> Attendances { get; set; } = new List<StudentAttendance>();
        public ICollection<SubjectMark> SubjectMarks { get; set; } = new List<SubjectMark>();
        public ICollection<ExamResult> ExamResults { get; set; } = new List<ExamResult>();
        public ICollection<StudentFee> StudentFees { get; set; } = new List<StudentFee>();
    }
}
