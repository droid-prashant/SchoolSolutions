using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Section
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<ClassSection> ClassSections{ get; set; }
    }
}
