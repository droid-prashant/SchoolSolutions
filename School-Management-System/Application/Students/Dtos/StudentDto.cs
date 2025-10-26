using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Students.Dtos
{
    public class StudentDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string GrandFatherName { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public int Gender { get; set; }
        public int Age { get; set; }
        public string ParentContactNumber { get; set; }
        public string ParentEmail { get; set; }
        public DateTime Dob { get; set; }
        public string Municipality { get; set; }
        public int WardNo { get; set; }
        public string Address { get; set; }
        public string ContactNumber { get; set; }
        public string ClassSectionId { get; set; }
    }
}
