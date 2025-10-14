using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ClassSections.Dtos
{
    public class ClassSectionDto
    {
        public Guid ClassRoomId { get; set; }
        public Guid[] SectionIdList { get; set; }
    }
}
