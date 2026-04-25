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
    public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
    {
        public void Configure(EntityTypeBuilder<Teacher> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.MiddleName).HasMaxLength(100);
            builder.Property(x => x.LastName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.EmployeeCode).HasMaxLength(50);
            builder.Property(x => x.ContactNumber).IsRequired().HasMaxLength(20);
            builder.Property(x => x.Email).HasMaxLength(150);
            builder.Property(x => x.Designation).HasMaxLength(100);
            builder.HasIndex(x => x.EmployeeCode).IsUnique().HasFilter("\"EmployeeCode\" IS NOT NULL AND \"EmployeeCode\" <> ''");
            builder.HasIndex(x => x.Email).IsUnique().HasFilter("\"Email\" IS NOT NULL AND \"Email\" <> ''");
            builder.HasMany(x => x.TeacherClassSections)
                   .WithOne(x => x.Teacher)
                   .HasForeignKey(x => x.TeacherId);
            builder.HasMany(x => x.Qualifications)
                   .WithOne(x => x.Teacher)
                   .HasForeignKey(x => x.TeacherId);
            builder.HasMany(x => x.Experiences)
                   .WithOne(x => x.Teacher)
                   .HasForeignKey(x => x.TeacherId);
            builder.HasMany(x => x.Documents)
                   .WithOne(x => x.Teacher)
                   .HasForeignKey(x => x.TeacherId);

        }
    }
}
