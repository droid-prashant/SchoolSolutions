namespace Application.Guardians.ViewModels
{
    public class GuardianViewModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string RelationType { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int LinkedStudentsCount { get; set; }
    }

    public class StudentGuardianViewModel
    {
        public Guid GuardianStudentId { get; set; }
        public Guid GuardianId { get; set; }
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string RelationType { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public bool IsGuardianActive { get; set; }
        public bool IsPrimaryGuardian { get; set; }
        public bool CanViewFees { get; set; }
        public bool CanViewResults { get; set; }
        public bool CanViewAttendance { get; set; }
        public bool CanPayFees { get; set; }
    }

    public class GuardianLinkedStudentViewModel
    {
        public Guid StudentId { get; set; }
        public Guid? StudentEnrollmentId { get; set; }
        public Guid? AcademicYearId { get; set; }
        public Guid? ClassSectionId { get; set; }
        public Guid GuardianStudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public int? RollNumber { get; set; }
        public bool CanViewFees { get; set; }
        public bool CanViewResults { get; set; }
        public bool CanViewAttendance { get; set; }
        public bool CanPayFees { get; set; }
    }
}
