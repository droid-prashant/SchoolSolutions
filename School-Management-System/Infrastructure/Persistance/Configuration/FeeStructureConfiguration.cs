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
    public class FeeStructureConfiguration : IEntityTypeConfiguration<FeeStructure>
    {
        public void Configure(EntityTypeBuilder<FeeStructure> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasMany(x => x.StudentFees)
                   .WithOne(x => x.FeeStructure)
                   .HasForeignKey(x => x.FeeStructureId);

        }
    }
}
