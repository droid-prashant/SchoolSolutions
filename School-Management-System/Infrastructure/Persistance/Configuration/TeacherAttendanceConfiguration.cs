using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.Configuration
{
    public class TeacherAttendanceConfiguration : IEntityTypeConfiguration<TeacherAttendance>
    {
        public void Configure(EntityTypeBuilder<TeacherAttendance> builder)
        {
            builder.Property(x => x.Status)
                   .HasConversion<int>();

            builder.Property(x => x.AttendanceDateNp)
                   .HasMaxLength(20);

            builder.Property(x => x.Remarks)
                   .HasMaxLength(500);

            builder.HasOne(x => x.Teacher)
                   .WithMany(x => x.Attendances)
                   .HasForeignKey(x => x.TeacherId);

            builder.HasOne(x => x.AcademicYear)
                   .WithMany(x => x.TeacherAttendances)
                   .HasForeignKey(x => x.AcademicYearId);

            builder.HasIndex(x => new { x.TeacherId, x.AcademicYearId, x.AttendanceDateEn })
                   .IsUnique()
                   .HasFilter("\"IsDeleted\" = false");

            builder.HasIndex(x => new { x.AcademicYearId, x.AttendanceDateEn });

            builder.HasIndex(x => new { x.TeacherId, x.AcademicYearId });

            builder.HasIndex(x => new { x.Status, x.AttendanceDateEn });
        }
    }
}
