using Domain;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.Configuration
{
    public class UserNotificationConfiguration : IEntityTypeConfiguration<UserNotification>
    {
        public void Configure(EntityTypeBuilder<UserNotification> builder)
        {
            builder.Property(x => x.DeliveryStatus)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.IsRead)
                .HasDefaultValue(false);

            builder.Property(x => x.IsPushSent)
                .HasDefaultValue(false);

            builder.Property(x => x.IsSignalRSent)
                .HasDefaultValue(false);

            builder.HasOne(x => x.Notification)
                .WithMany(x => x.UserNotifications)
                .HasForeignKey(x => x.NotificationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.NotificationId);
            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.IsRead);
            builder.HasIndex(x => x.CreatedAt);
            builder.HasIndex(x => new { x.UserId, x.IsRead, x.CreatedAt });
            builder.HasIndex(x => new { x.NotificationId, x.UserId }).IsUnique();
        }
    }
}
