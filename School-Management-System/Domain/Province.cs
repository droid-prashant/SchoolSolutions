using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Province
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string ProvinceName { get; set; }
        public string ProvinceNameNp { get; set; }
        public bool? IsActive { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public ICollection<District> Districts { get; set; }
    }
}
