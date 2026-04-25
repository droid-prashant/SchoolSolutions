using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Teacher:AuditableEntry
    {
        public Guid? UserId { get; set; }
        public string EmployeeCode { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public int Gender { get; set; }
        public int? Age { get; set; }
        public string DateOfBirthNp { get; set; }
        public string DateOfBirthEn { get; set; }
        public string ContactNumber { get; set; }
        public string AlternateContactNumber { get; set; }
        public string Email { get; set; }
        public int? ProvinceId { get; set; }
        public int? DistrictId { get; set; }
        public int? MunicipalityId { get; set; }
        public int? WardNo { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string Designation { get; set; }
        public string JoiningDateNp { get; set; }
        public string JoiningDateEn { get; set; }
        public string? InactiveReason { get; set; }
        public DateTime? InactiveDate { get; set; }
        public ICollection<TeacherClassSection> TeacherClassSections { get; set; } = new List<TeacherClassSection>();
        public ICollection<TeacherQualification> Qualifications { get; set; } = new List<TeacherQualification>();
        public ICollection<TeacherExperience> Experiences { get; set; } = new List<TeacherExperience>();
        public ICollection<TeacherDocument> Documents { get; set; } = new List<TeacherDocument>();
    }
}
