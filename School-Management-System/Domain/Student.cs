using System.Collections.ObjectModel;

namespace Domain
{
    public class Student:AuditableEntry
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
        public DateTime DateOfBirth { get; set; }
        public string Municipality { get; set; }
        public int WardNo { get; set; }
        public string Address { get; set; }
        public string ContactNumber { get; set; }
        public ICollection<StudentEnrollment> StudentEnrollments { get; set; } = new List<StudentEnrollment>();
    }
}
