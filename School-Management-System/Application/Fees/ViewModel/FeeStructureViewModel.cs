using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;

namespace Application.Fees.ViewModel
{
    public class FeeStructureViewModel
    {
        public Guid Id { get; set; }
        public string FeeTypeId{ get; set; }
        public string FeeType { get; set; }
        public string ClassId { get; set; }
        public string Class { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }
}
