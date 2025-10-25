using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;

namespace Application.Fees.ViewModel
{
    public class FeeTypeViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string IsRecurring { get; set; }
        public string Frequency { get; set; }
    }
}
