using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.Configuration
{
    public class TeacherExperienceConfiguration : IEntityTypeConfiguration<TeacherExperience>
    {
        public void Configure(EntityTypeBuilder<TeacherExperience> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.OrganizationName).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Designation).IsRequired().HasMaxLength(100);
            builder.Property(x => x.SubjectOrDepartment).HasMaxLength(100);
            builder.Property(x => x.Remarks).HasMaxLength(300);
        }
    }
}
