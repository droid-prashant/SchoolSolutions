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

            builder.HasMany(x => x.SubjectMarks)
                   .WithOne(x => x.StudentEnrollment)
                   .HasForeignKey(x => x.StudentEnrollmentId);

            builder.HasMany(x => x.StudentFees)
                   .WithOne(x => x.StudentEnrollment)
                   .HasForeignKey(x => x.StudentEnrollmentId);
        }
    }
}
