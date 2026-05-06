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
    public class StudentEnrollmentConfiguration : IEntityTypeConfiguration<StudentEnrollment>
    {
        public void Configure(EntityTypeBuilder<StudentEnrollment> builder)
        {
            builder.Property(x => x.EnrollmentStatus)
                   .HasConversion<int>();

            builder.Property(x => x.StatusRemarks)
                   .HasMaxLength(500);

            builder.HasIndex(x => new { x.AcademicYearId, x.ClassSectionId, x.IsActive, x.IsDeleted });

            builder.HasIndex(x => new { x.StudentId, x.AcademicYearId });

            builder.HasMany(x => x.SubjectMarks)
                   .WithOne(x => x.StudentEnrollment)
                   .HasForeignKey(x => x.StudentEnrollmentId);

            builder.HasMany(x => x.StudentFees)
                   .WithOne(x => x.StudentEnrollment)
                   .HasForeignKey(x => x.StudentEnrollmentId);
        }
    }
}
