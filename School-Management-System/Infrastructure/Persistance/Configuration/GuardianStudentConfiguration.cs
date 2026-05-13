using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.Configuration
{
    public class GuardianStudentConfiguration : IEntityTypeConfiguration<GuardianStudent>
    {
        public void Configure(EntityTypeBuilder<GuardianStudent> builder)
        {
            builder.HasOne(x => x.Guardian)
                .WithMany(x => x.GuardianStudents)
                .HasForeignKey(x => x.GuardianId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Student)
                .WithMany()
                .HasForeignKey(x => x.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.GuardianId);
            builder.HasIndex(x => x.StudentId);
            builder.HasIndex(x => new { x.GuardianId, x.StudentId });
        }
    }
}
