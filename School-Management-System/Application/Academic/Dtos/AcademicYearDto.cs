using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Academic.Dtos
{
    public class AcademicYearDto
    {
        public string YearName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string StartDateNp { get; set; }
        public string EndDateNp { get; set; }
        public string StartDateEn { get; set; }
        public string EndDateEn { get; set; }
    }
}
