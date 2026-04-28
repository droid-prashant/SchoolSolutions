namespace Application.Teachers
{
    public interface ITeacherService
    {
        Task<List<TeacherViewModel>> GetTeachersAsync(string? academicYearId, bool includeInactive, CancellationToken cancellationToken);
        Task<TeacherViewModel?> GetTeacherByIdAsync(string teacherId, CancellationToken cancellationToken);
        Task AddTeacherAsync(TeacherDto teacherDto, CancellationToken cancellationToken);
        Task UpdateTeacherAsync(TeacherDto teacherDto, CancellationToken cancellationToken);
        Task UpdateTeacherStatusAsync(string teacherId, TeacherStatusDto teacherStatusDto, CancellationToken cancellationToken);
        Task SoftDeleteTeacherAsync(string teacherId, CancellationToken cancellationToken);
        Task<List<TeacherClassSectionViewModel>> GetTeacherAssignmentsAsync(string teacherId, string? academicYearId, CancellationToken cancellationToken);
        Task AddTeacherAssignmentAsync(string teacherId, TeacherClassSectionDto assignment, CancellationToken cancellationToken);
        Task UpdateTeacherAssignmentAsync(string teacherId, TeacherClassSectionDto assignment, CancellationToken cancellationToken);
        Task DeleteTeacherAssignmentAsync(string assignmentId, CancellationToken cancellationToken);
        Task CopyTeacherAssignmentsAsync(string teacherId, TeacherAssignmentCopyDto teacherAssignmentCopyDto, CancellationToken cancellationToken);
        Task<TeacherDocumentViewModel> AddTeacherDocumentAsync(TeacherDocumentDto teacherDocumentDto, CancellationToken cancellationToken);
        Task<TeacherDocumentViewModel?> GetTeacherDocumentAsync(string documentId, CancellationToken cancellationToken);
        Task DeleteTeacherDocumentAsync(string documentId, CancellationToken cancellationToken);
        Task<TeacherAccountViewModel> GetTeacherAccountAsync(string teacherId, CancellationToken cancellationToken);
        Task CreateTeacherUserAsync(string teacherId, TeacherAccountCreateDto teacherAccountCreateDto, CancellationToken cancellationToken);
        Task UpdateTeacherUserStatusAsync(string teacherId, TeacherAccountStatusDto teacherAccountStatusDto, CancellationToken cancellationToken);
        Task ResetTeacherUserPasswordAsync(string teacherId, TeacherPasswordResetDto teacherPasswordResetDto, CancellationToken cancellationToken);
        Task<TeacherDashboardViewModel> GetTeacherDashboardAsync(string? academicYearId, CancellationToken cancellationToken);
    }
}
