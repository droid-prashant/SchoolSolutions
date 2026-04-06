using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Students.ViewModels
{
    public class StudentCertificateViewModel
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public string AcademicYear { get; set; }
        public DateTime AdmittedDate { get; set; }
        public DateTime IssueDate { get; set; }
        public string Name { get; set; }
        public int? Age { get; set; }
        public int Gender { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string DateOfBirth { get; set; }
        public string Address { get; set; }
        public string? RegistrationNumber { get; set; }
        public string? SymbolNumber { get; set; }
        public int WardNo { get; set; }
        public string ClassRoom { get; set; }
        public string FirstAdmittedClass { get; set; }
        public string Section { get; set; }
        public decimal GPA { get; set; }
        public int ExamType { get; set; }
        public DateTime ExamHeld { get; set; }
        public bool IsCharacterCertificateTaken { get; set; }
        public bool IsTransferCertificateTaken { get; set; }
    }
}
