using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Academic.ViewModels
{
    public class AcademicViewModels
    {
        public string Id { get; set; } = string.Empty;
        public string YearName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string StartDateNp { get; set; }
        public string EndDateNp { get; set; }
        public string StartDateEn { get; set; }
        public string EndDateEn { get; set; }
        public string StartDateFormatted { get; set; } =string.Empty;
        public string EndDateFormatted { get; set; } = string.Empty ;
    }
}
