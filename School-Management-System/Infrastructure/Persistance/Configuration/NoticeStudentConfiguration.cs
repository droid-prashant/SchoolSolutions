using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.Configuration
{
    public class NoticeStudentConfiguration : IEntityTypeConfiguration<NoticeStudent>
    {
        public void Configure(EntityTypeBuilder<NoticeStudent> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Notice)
                .WithMany(x => x.NoticeStudents)
                .HasForeignKey(x => x.NoticeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Student)
                .WithMany()
                .HasForeignKey(x => x.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.NoticeId);
            builder.HasIndex(x => x.StudentId);
            builder.HasIndex(x => new { x.NoticeId, x.StudentId }).IsUnique();
        }
    }
}
