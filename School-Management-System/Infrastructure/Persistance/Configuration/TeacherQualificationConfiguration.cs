using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.Configuration
{
    public class TeacherQualificationConfiguration : IEntityTypeConfiguration<TeacherQualification>
    {
        public void Configure(EntityTypeBuilder<TeacherQualification> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.DegreeName).IsRequired().HasMaxLength(150);
            builder.Property(x => x.InstitutionName).IsRequired().HasMaxLength(200);
            builder.Property(x => x.BoardOrUniversity).HasMaxLength(150);
            builder.Property(x => x.PassedYear).HasMaxLength(20);
            builder.Property(x => x.GradeOrPercentage).HasMaxLength(50);
            builder.Property(x => x.MajorSubject).HasMaxLength(100);
            builder.Property(x => x.Remarks).HasMaxLength(300);
            builder.HasIndex(x => new { x.TeacherId, x.DegreeName, x.InstitutionName, x.PassedYear, x.IsDeleted });
        }
    }
}
