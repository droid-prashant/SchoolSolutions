namespace Application.Fees.Dtos
{
    public class PreviousYearDueViewModel
    {
        public string AcademicYearName { get; set; } = string.Empty;
        public decimal PendingAmount { get; set; }
    }
}
