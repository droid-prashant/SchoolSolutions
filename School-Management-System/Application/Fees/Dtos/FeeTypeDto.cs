using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;

namespace Application.Fees.Dtos
{
    public class FeeTypeDto
    {
        public string Name { get; set; }
        public bool IsRecurring { get; set; }
        public string Frequency { get; set; }
    }
}
