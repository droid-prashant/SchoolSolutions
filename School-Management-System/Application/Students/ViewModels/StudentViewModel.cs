using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;

namespace Application.Students.ViewModels
{
    public class StudentViewModel
    {
        public Guid Id { get; set; }
        public Guid StudentEnrollmentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? Age { get; set; }
        public int Gender { get; set; }
        public string GrandFatherName { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string DateOfBirthNp { get; set; }
        public string DateOfBirthEn { get; set; }
        public string ContactNumber { get; set; }
        public string? ParentEmail { get; set; }
        public string ParentContactNumber { get; set; }
        public int ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public int DistrictId { get; set; }
        public string DistrictName { get; set; }
        public int MunicipalityId { get; set; }
        public string MunicipalityName { get; set; }
        public string? RegistrationNumber { get; set; }
        public string? SymbolNumber { get; set; }
        public int WardNo { get; set; }
        public string Address { get; set; }
        public string ClassRoomId { get; set; }
        public string ClassSectionId { get; set; }
        public string ClassRoomName { get; set; }
        public string SectionId { get; set; }
        public string SectionName { get; set; }
        public int RollNumber { get; set; }
        public bool IsActive { get; set; }
        public int EnrollmentStatus { get; set; }
        public string EnrollmentStatusName { get; set; } = string.Empty;
        public DateTime? StatusDate { get; set; }
        public string? StatusRemarks { get; set; }
    }
}
