using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class District
    {
        public int Id { get; set; }
        public string DistrictName { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int ProvinceId { get; set; }
        public Province Province { get; set; } = null!;
        public ICollection<Municipality> Municipalities { get; set; }
    }
}
