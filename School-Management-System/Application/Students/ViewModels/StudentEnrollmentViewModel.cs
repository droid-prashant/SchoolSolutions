using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Students.ViewModels
{
    public class StudentEnrollmentViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Class { get; set; }
        public string ClassSectionId { get; set; }
        public string  RegistrationNumber { get; set; }
        public string  SymbolNumber { get; set; }
    }
}
