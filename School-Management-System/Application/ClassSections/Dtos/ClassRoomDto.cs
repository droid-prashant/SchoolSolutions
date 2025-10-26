using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Application.ClassSections.Dtos
{
    public class ClassRoomDto
    {
        public string? Id { get; set; }
        public string Name { get; set; }
        public int OrderNumber { get; set; }
    }
}
