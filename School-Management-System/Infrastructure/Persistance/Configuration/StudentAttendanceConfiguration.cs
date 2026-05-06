using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.Configuration
{
    public class StudentAttendanceConfiguration : IEntityTypeConfiguration<StudentAttendance>
    {
        public void Configure(EntityTypeBuilder<StudentAttendance> builder)
        {
            builder.Property(x => x.Status)
                   .HasConversion<int>();

            builder.Property(x => x.AttendanceDateNp)
                   .HasMaxLength(20);

            builder.Property(x => x.Remarks)
                   .HasMaxLength(500);

            builder.HasOne(x => x.StudentEnrollment)
                   .WithMany(x => x.Attendances)
                   .HasForeignKey(x => x.StudentEnrollmentId);

            builder.HasOne(x => x.Student)
                   .WithMany(x => x.Attendances)
                   .HasForeignKey(x => x.StudentId);

            builder.HasOne(x => x.ClassSection)
                   .WithMany(x => x.StudentAttendances)
                   .HasForeignKey(x => x.ClassSectionId);

            builder.HasOne(x => x.AcademicYear)
                   .WithMany(x => x.StudentAttendances)
                   .HasForeignKey(x => x.AcademicYearId);

            builder.HasIndex(x => new { x.StudentEnrollmentId, x.AttendanceDateEn })
                   .IsUnique()
                   .HasFilter("\"IsDeleted\" = false");

            builder.HasIndex(x => new { x.AcademicYearId, x.ClassSectionId, x.AttendanceDateEn });

            builder.HasIndex(x => new { x.StudentEnrollmentId, x.AttendanceDateEn, x.IsDeleted });

            builder.HasIndex(x => new { x.StudentId, x.AcademicYearId });

            builder.HasIndex(x => new { x.Status, x.AttendanceDateEn });
        }
    }
}
