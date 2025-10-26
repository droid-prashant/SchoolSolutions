using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.Configuration
{
    public class ClassSectionConfiguration : IEntityTypeConfiguration<ClassSection>
    {
        public void Configure(EntityTypeBuilder<ClassSection> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasMany(x => x.StudentEnrollments)
                   .WithOne(x => x.ClassSection)
                   .HasForeignKey(x => x.ClassSectionId);

            builder.HasMany(x => x.TeacherClassSections)
                   .WithOne(x => x.ClassSection)
                   .HasForeignKey(x => x.ClassSectionId);

        }
    }
}
