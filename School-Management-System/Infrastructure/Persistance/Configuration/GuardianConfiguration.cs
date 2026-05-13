using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.Configuration
{
    public class GuardianConfiguration : IEntityTypeConfiguration<Guardian>
    {
        public void Configure(EntityTypeBuilder<Guardian> builder)
        {
            builder.Property(x => x.FullName)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(x => x.ContactNumber)
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(x => x.Email)
                .HasMaxLength(150);

            builder.Property(x => x.RelationType)
                .HasMaxLength(50)
                .IsRequired();

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.ContactNumber);
            builder.HasIndex(x => x.Email);
        }
    }
}
