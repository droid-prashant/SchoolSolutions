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
    public class TeacherClassSectionConfiguration : IEntityTypeConfiguration<TeacherClassSection>
    {
        public void Configure(EntityTypeBuilder<TeacherClassSection> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Remarks).HasMaxLength(300);
            builder.HasOne(x => x.Teacher)
                   .WithMany(x => x.TeacherClassSections)
                   .HasForeignKey(x => x.TeacherId);
            builder.HasOne(x => x.ClassSection)
                   .WithMany(x => x.TeacherClassSections)
                   .HasForeignKey(x => x.ClassSectionId);
            builder.HasOne(x => x.Course)
                   .WithMany(x => x.TeacherClassSections)
                   .HasForeignKey(x => x.CourseId);
            builder.HasOne(x => x.AcademicYear)
                   .WithMany(x => x.TeacherClassSections)
                   .HasForeignKey(x => x.AcademicYearId);
            builder.HasIndex(x => new { x.AcademicYearId, x.ClassSectionId, x.CourseId, x.IsDeleted });
            builder.HasIndex(x => new { x.TeacherId, x.AcademicYearId, x.ClassSectionId, x.CourseId, x.IsDeleted });
            builder.HasIndex(x => new { x.AcademicYearId, x.ClassSectionId, x.IsClassTeacher, x.IsActive, x.IsDeleted });
        }
    }
}
