using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class TeacherClassSection : AuditableEntry
    {
        public Teacher Teacher { get; set; }
        public Guid TeacherId { get; set; }
        public AcademicYear AcademicYear { get; set; }
        public Guid AcademicYearId { get; set; }
        public ClassSection ClassSection { get; set; }
        public Guid ClassSectionId { get; set; }
        public Course Course { get; set; }
        public Guid CourseId { get; set; }
        public bool IsClassTeacher { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string Remarks { get; set; }
    }
}
