using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.Configuration
{
    internal class FeeTypesConfiguration : IEntityTypeConfiguration<FeeType>
    {
        public void Configure(EntityTypeBuilder<FeeType> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(120);

            builder.Property(x => x.Frequency)
                   .HasMaxLength(40);

            builder.Property(x => x.Applicability)
                   .HasConversion<int>()
                   .HasDefaultValue(FeeApplicability.Standard);

            builder.HasIndex(x => x.Name)
                   .IsUnique();

            builder.HasMany(x => x.FeeStructures)
                   .WithOne(x => x.FeeType)
                   .HasForeignKey(x => x.FeeTypeId);
        }
    }
}
