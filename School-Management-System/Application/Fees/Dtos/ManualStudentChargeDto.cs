namespace Application.Fees.Dtos
{
    public class ManualStudentChargeDto
    {
        public Guid StudentEnrollmentId { get; set; }
        public Guid FeeStructureId { get; set; }
        public decimal? Amount { get; set; }
    }
}
