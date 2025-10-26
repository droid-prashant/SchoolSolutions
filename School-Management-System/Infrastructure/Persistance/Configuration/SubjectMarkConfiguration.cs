using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.Configuration
{
    public class SubjectMarkConfiguration : IEntityTypeConfiguration<SubjectMark>
    {
        public void Configure(EntityTypeBuilder<SubjectMark> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.StudentEnrollment)
                   .WithMany(x => x.SubjectMarks)
                   .HasForeignKey(x => x.StudentEnrollmentId);

            builder.HasOne(x => x.ClassCourse)
                   .WithMany(x=>x.SubjectMarks)
                   .HasForeignKey(x => x.ClassCourseId);
        }
    }
}
