using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;

namespace Application.Fees.Dtos
{
    public class FeeStructureDto
    {
        public Guid AcademicYearId { get; set; }
        public Guid FeeTypeId { get; set; }
        public Guid ClassId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }
}
