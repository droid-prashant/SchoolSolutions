using Application.Guardians.Dtos;
using Application.Guardians.ViewModels;

namespace Application.Guardians.Interfaces
{
    public interface IGuardianService
    {
        Task<List<StudentGuardianViewModel>> GetStudentGuardiansAsync(string studentId, CancellationToken cancellationToken);
        Task<List<GuardianViewModel>> SearchGuardiansAsync(string? keyword, CancellationToken cancellationToken);
        Task<StudentGuardianViewModel> CreateGuardianForStudentAsync(string studentId, GuardianCreateDto guardianCreateDto, CancellationToken cancellationToken);
        Task<StudentGuardianViewModel> LinkGuardianToStudentAsync(GuardianStudentLinkDto guardianStudentLinkDto, CancellationToken cancellationToken);
        Task UpdateStudentGuardianAccessAsync(string guardianStudentId, GuardianStudentAccessDto guardianStudentAccessDto, CancellationToken cancellationToken);
        Task UnlinkGuardianFromStudentAsync(string guardianStudentId, CancellationToken cancellationToken);
        Task<List<GuardianLinkedStudentViewModel>> GetCurrentGuardianStudentsAsync(CancellationToken cancellationToken);
        Task EnsureCurrentGuardianCanViewFeesAsync(string studentEnrollmentId, CancellationToken cancellationToken);
        Task EnsureCurrentGuardianCanViewAttendanceAsync(string studentId, CancellationToken cancellationToken);
        Task EnsureCurrentGuardianCanViewResultsAsync(string studentEnrollmentId, CancellationToken cancellationToken);
    }
}
