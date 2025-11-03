using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistance.Configuration
{
    public class DistrictConfiguration : IEntityTypeConfiguration<District>
    {
        void IEntityTypeConfiguration<District>.Configure(EntityTypeBuilder<District> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                   .ValueGeneratedOnAdd();

            builder.HasMany(x => x.Municipalities)
                   .WithOne(x => x.District)
                   .HasForeignKey(x => x.DistrictId);
        }
    }
}
