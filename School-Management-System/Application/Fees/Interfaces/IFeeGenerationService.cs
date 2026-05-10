namespace Application.Fees.Interfaces
{
    public interface IFeeGenerationService
    {
        Task EnsureFeesForEnrollmentAsync(Guid studentEnrollmentId, Guid academicYearId, CancellationToken cancellationToken);
        Task SyncFeesForClassAsync(Guid classId, Guid academicYearId, CancellationToken cancellationToken);
    }
}
