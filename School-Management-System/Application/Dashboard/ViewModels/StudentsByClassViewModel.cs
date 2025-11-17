using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dashboard.ViewModels
{
    public class StudentsByClassViewModel
    {
        public string ClassRoom { get; set; }
        public List<StudentCountBySection> StudentsCountBySections { get; set; }
    }
}
