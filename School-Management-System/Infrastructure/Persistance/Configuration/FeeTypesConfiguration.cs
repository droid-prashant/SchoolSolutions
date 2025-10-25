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
    internal class FeeTypesConfiguration : IEntityTypeConfiguration<FeeType>
    {
        public void Configure(EntityTypeBuilder<FeeType> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasMany(x => x.FeeStructures)
                   .WithOne(x => x.FeeType)
                   .HasForeignKey(x => x.FeeTypeId);
        }
    }
}
