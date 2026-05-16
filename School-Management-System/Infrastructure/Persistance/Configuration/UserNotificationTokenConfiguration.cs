using Domain;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.Configuration
{
    public class UserNotificationTokenConfiguration : IEntityTypeConfiguration<UserNotificationToken>
    {
        public void Configure(EntityTypeBuilder<UserNotificationToken> builder)
        {
            builder.Property(x => x.FcmToken).IsRequired();
            builder.Property(x => x.DeviceType).HasMaxLength(50);
            builder.Property(x => x.Browser).HasMaxLength(100);
            builder.Property(x => x.Platform).HasMaxLength(100);
            builder.Property(x => x.IsActive).HasDefaultValue(true);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.FcmToken);
            builder.HasIndex(x => new { x.UserId, x.FcmToken, x.IsActive })
                .IsUnique()
                .HasFilter("\"IsActive\" = true");
        }
    }
}
