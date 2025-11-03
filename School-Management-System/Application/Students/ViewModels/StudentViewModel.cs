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
        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string DateOfBirth { get; set; }
        public string ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public string DistrictId { get; set; }
        public string DistrictName { get; set; }
        public string MunicipalityId { get; set; }
        public string MunicipalityName { get; set; }
        public string? RegistrationNumber { get; set; }
        public string? SymbolNumber { get; set; }
        public int WardNo { get; set; }
        public string Address { get; set; }
        public string ClassRoom { get; set; }
        public string Section { get; set; }
    }
}
