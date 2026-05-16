using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.Configuration
{
    public class NoticeConfiguration : IEntityTypeConfiguration<Notice>
    {
        public void Configure(EntityTypeBuilder<Notice> builder)
        {
            builder.Property(x => x.Title)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Description)
                .IsRequired();

            builder.Property(x => x.NoticeDateNp)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.TargetAudience)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.IsPublished)
                .HasDefaultValue(false);

            builder.Property(x => x.IsDeleted)
                .HasDefaultValue(false);

            builder.HasOne(x => x.Class)
                .WithMany()
                .HasForeignKey(x => x.ClassId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Section)
                .WithMany()
                .HasForeignKey(x => x.SectionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.ClassId);
            builder.HasIndex(x => x.SectionId);
            builder.HasIndex(x => x.TargetAudience);
            builder.HasIndex(x => x.NoticeDate);
            builder.HasIndex(x => x.CreatedAt);
            builder.HasIndex(x => x.IsPublished);
        }
    }
}
