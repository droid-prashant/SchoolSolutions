namespace Application.Teachers
{
    public class TeacherViewModel
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string? EmployeeCode { get; set; }
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
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
        public bool IsActive { get; set; }
        public string? InactiveReason { get; set; }
        public List<TeacherQualificationViewModel> Qualifications { get; set; } = new();
        public List<TeacherExperienceViewModel> Experiences { get; set; } = new();
        public List<TeacherDocumentViewModel> Documents { get; set; } = new();
        public List<TeacherClassSectionViewModel> Assignments { get; set; } = new();
    }

    public class TeacherQualificationViewModel
    {
        public Guid Id { get; set; }
        public string DegreeName { get; set; }
        public string InstitutionName { get; set; }
        public string? BoardOrUniversity { get; set; }
        public string? PassedYear { get; set; }
        public string? GradeOrPercentage { get; set; }
        public string? MajorSubject { get; set; }
        public string? Remarks { get; set; }
    }

    public class TeacherExperienceViewModel
    {
        public Guid Id { get; set; }
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

    public class TeacherClassSectionViewModel
    {
        public Guid Id { get; set; }
        public Guid AcademicYearId { get; set; }
        public string AcademicYearName { get; set; }
        public Guid ClassSectionId { get; set; }
        public Guid ClassRoomId { get; set; }
        public string ClassRoomName { get; set; }
        public Guid SectionId { get; set; }
        public string SectionName { get; set; }
        public Guid CourseId { get; set; }
        public string CourseName { get; set; }
        public bool IsClassTeacher { get; set; }
        public bool IsActive { get; set; }
        public string? Remarks { get; set; }
    }

    public class TeacherDocumentViewModel
    {
        public Guid Id { get; set; }
        public string DocumentType { get; set; }
        public string DocumentTitle { get; set; }
        public string FilePath { get; set; }
        public string OriginalFileName { get; set; }
        public string MimeType { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadedDate { get; set; }
    }

    public class TeacherDashboardViewModel
    {
        public int TotalActiveTeachers { get; set; }
        public int TotalInactiveTeachers { get; set; }
        public int AssignedTeachers { get; set; }
        public int ClassTeachers { get; set; }
    }

    public class TeacherAccountViewModel
    {
        public bool IsAccountCreated { get; set; }
        public Guid? UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
