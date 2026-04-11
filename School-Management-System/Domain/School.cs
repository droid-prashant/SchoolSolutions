using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class School:AuditableEntry
    {
        public string Name { get; set; }
        public DateOnly EstablishedYear { get; set; }
        public string PrincipalName { get; set; } = string.Empty;
        public string Province { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public int WardNo { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string LogoUrl { get; set; }
    }
}
