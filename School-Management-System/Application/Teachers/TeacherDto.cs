namespace Application.Teachers
{
    public class TeacherDto
    {
        public string? Id { get; set; }
        public string? EmployeeCode { get; set; }
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string LastName { get; set; }
        public int Gender { get; set; }
        public int? Age { get; set; }
        public string? DateOfBirthNp { get; set; }
        public string? DateOfBirthEn { get; set; }
        public string ContactNumber { get; set; }
        public string? AlternateContactNumber { get; set; }
        public string? Email { get; set; }
        public int? ProvinceId { get; set; }
        public int? DistrictId { get; set; }
        public int? MunicipalityId { get; set; }
        public int? WardNo { get; set; }
        public string? FatherName { get; set; }
        public string? MotherName { get; set; }
        public string? Designation { get; set; }
        public string? JoiningDateNp { get; set; }
        public string? JoiningDateEn { get; set; }
        public bool CreateUser { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public List<TeacherQualificationDto> Qualifications { get; set; } = new();
        public List<TeacherExperienceDto> Experiences { get; set; } = new();
        public List<TeacherClassSectionDto> Assignments { get; set; } = new();
    }

    public class TeacherQualificationDto
    {
        public string? Id { get; set; }
        public string DegreeName { get; set; }
        public string InstitutionName { get; set; }
        public string? BoardOrUniversity { get; set; }
        public string? PassedYear { get; set; }
        public string? GradeOrPercentage { get; set; }
        public string? MajorSubject { get; set; }
        public string? Remarks { get; set; }
    }

    public class TeacherExperienceDto
    {
        public string? Id { get; set; }
        public string OrganizationName { get; set; }
        public string Designation { get; set; }
        public string? SubjectOrDepartment { get; set; }
        public string? StartDateNp { get; set; }
        public string? StartDateEn { get; set; }
        public string? EndDateNp { get; set; }
        public string? EndDateEn { get; set; }
        public bool IsCurrent { get; set; }
        public string? Remarks { get; set; }
    }

    public class TeacherClassSectionDto
    {
        public string? Id { get; set; }
        public string AcademicYearId { get; set; }
        public string ClassSectionId { get; set; }
        public string CourseId { get; set; }
        public bool IsClassTeacher { get; set; }
        public string? Remarks { get; set; }
    }

    public class TeacherStatusDto
    {
        public bool IsActive { get; set; }
        public string? InactiveReason { get; set; }
    }

    public class TeacherDocumentDto
    {
        public string TeacherId { get; set; }
        public string DocumentType { get; set; }
        public string DocumentTitle { get; set; }
        public string FilePath { get; set; }
        public string OriginalFileName { get; set; }
        public string MimeType { get; set; }
        public long FileSize { get; set; }
    }

    public class TeacherAccountCreateDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class TeacherAccountStatusDto
    {
        public bool IsActive { get; set; }
    }

    public class TeacherPasswordResetDto
    {
        public string NewPassword { get; set; }
    }

    public class TeacherAssignmentCopyDto
    {
        public string SourceAcademicYearId { get; set; }
        public string TargetAcademicYearId { get; set; }
    }
}
