using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class Student : AuditableEntry
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string GrandFatherName { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string ParentContactNumber { get; set; }
        public string ParentEmail { get; set; }
        public int Gender { get; set; }
        public int Age { get; set; }
        public string DateOfBirthNp { get; set; }
        public string DateOfBirthEn { get; set; }
        public int WardNo { get; set; }
        public string Address { get; set; }
        public string ContactNumber { get; set; }
        public int ProvinceId { get; set; }
        public Province Province { get; set; }
        public int DistrictId { get; set; }
        public District District { get; set; }
        public int MunicipalityId { get; set; }
        public bool isActive { get; set; }
        public Municipality Municipality { get; set; }
        public StudentCharacterCertificateLog StudentCharacterCertificateLog { get; set; }
        public StudentTransferCertificateLog studentTransferCertificateLog { get; set; }
        public ICollection<StudentEnrollment> StudentEnrollments { get; set; } = new List<StudentEnrollment>();
    }
}
