using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class Municipality
    {
        public int Id { get; set; }
        public string MunicipalityName { get; set; } = string.Empty;
        public string MunicipalityNameNp { get; set; } = string.Empty;
        public string? BudgetCode { get; set; }
        public string? Type { get; set; }
        public string Code { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string MunicipalityDisplayName { get; set; } = string.Empty;
        public string? Eco_Region { get; set; }
        public string? Eco_Region_Value { get; set; }
        public string? Cluster { get; set; }
        public int DistrictId { get; set; }
        public District District { get; set; } = null!;

    }
}
