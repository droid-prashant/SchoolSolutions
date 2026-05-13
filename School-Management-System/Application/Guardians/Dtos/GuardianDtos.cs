namespace Application.Guardians.Dtos
{
    public class GuardianCreateDto
    {
        public string FullName { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string RelationType { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool IsPrimaryGuardian { get; set; }
        public bool CanViewFees { get; set; } = true;
        public bool CanViewResults { get; set; } = true;
        public bool CanViewAttendance { get; set; } = true;
        public bool CanPayFees { get; set; }
    }

    public class GuardianStudentLinkDto
    {
        public string GuardianId { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public bool IsPrimaryGuardian { get; set; }
        public bool CanViewFees { get; set; } = true;
        public bool CanViewResults { get; set; } = true;
        public bool CanViewAttendance { get; set; } = true;
        public bool CanPayFees { get; set; }
    }

    public class GuardianStudentAccessDto
    {
        public bool IsPrimaryGuardian { get; set; }
        public bool CanViewFees { get; set; }
        public bool CanViewResults { get; set; }
        public bool CanViewAttendance { get; set; }
        public bool CanPayFees { get; set; }
    }
}
