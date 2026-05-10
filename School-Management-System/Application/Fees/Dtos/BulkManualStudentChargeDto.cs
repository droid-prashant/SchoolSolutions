namespace Application.Fees.Dtos
{
    public class BulkManualStudentChargeDto
    {
        public Guid ClassSectionId { get; set; }
        public Guid FeeStructureId { get; set; }
        public decimal? Amount { get; set; }
        public List<Guid> StudentEnrollmentIds { get; set; } = new();
    }
}
