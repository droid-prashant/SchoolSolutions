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
        Task<TeacherDocumentViewModel> AddTeacherDocumentAsync(TeacherDocumentDto teacherDocumentDto, CancellationToken cancellationToken);
        Task<TeacherDocumentViewModel?> GetTeacherDocumentAsync(string documentId, CancellationToken cancellationToken);
        Task DeleteTeacherDocumentAsync(string documentId, CancellationToken cancellationToken);
        Task<TeacherDashboardViewModel> GetTeacherDashboardAsync(string? academicYearId, CancellationToken cancellationToken);
    }
}
