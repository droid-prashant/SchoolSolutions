using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Domain.Enums;

namespace Domain
{
    public class FeeType : AuditableEntry
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsRecurring { get; set; }
        public string Frequency { get; set; }
        public FeeApplicability Applicability { get; set; } = FeeApplicability.Standard;
        public ICollection<FeeStructure> FeeStructures { get; set; }
    }
}
