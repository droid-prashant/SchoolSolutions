using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.Configuration
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.Property(x => x.Title)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Message)
                .IsRequired();

            builder.Property(x => x.NotificationType)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.NotificationDateTimeNp)
                .HasMaxLength(40)
                .IsRequired();

            builder.Property(x => x.IsDeleted)
                .HasDefaultValue(false);

            builder.HasOne(x => x.Student)
                .WithMany()
                .HasForeignKey(x => x.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.StudentId);
            builder.HasIndex(x => x.NotificationType);
            builder.HasIndex(x => x.ReferenceId);
            builder.HasIndex(x => x.CreatedAt);
            builder.HasIndex(x => new { x.NotificationType, x.ReferenceId, x.StudentId });
        }
    }
}
