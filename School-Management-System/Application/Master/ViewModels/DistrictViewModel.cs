using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Master.ViewModels
{
    public class DistrictViewModel
    {
        public int Id{ get; set; }
        public string Name { get; set; }
        public List<MunicipalityViewModel> Municipalities { get; set; }
    }
}
