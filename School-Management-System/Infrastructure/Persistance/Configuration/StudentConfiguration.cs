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
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.HasMany(x => x.ExamResults)
                   .WithOne(x => x.Student)
                   .HasForeignKey(x=>x.StudentId);

            builder.HasMany(x => x.SubjectMarks)
                   .WithOne(x => x.Student)
                   .HasForeignKey(x => x.StudentId);
        }
    }
}
