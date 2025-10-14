using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;

namespace Application.Courses.ViewModels
{
    public class CourseCreditMarkViewModel
    {
        public decimal TheoryCreditHour { get; set; }
        public decimal PracticalCreditHour { get; set; }
        public decimal TheoryFullMarks { get; set; }
        public decimal PracticalFullMarks { get; set; }
    }
}
