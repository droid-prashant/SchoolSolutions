using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ClassSections.VieModels
{
    public class ClassSectionViewModel
    {
        public Guid Id { get; set; }
        public Guid SectionId { get; set; }
        public string ClassRoomName { get; set; }
        public string Section { get; set; }
    }
}
