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
    public class SectionConfiguration : IEntityTypeConfiguration<Section>
    {
        public void Configure(EntityTypeBuilder<Section> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasMany(x => x.ClassSections)
                   .WithOne(x => x.Section)
                   .HasForeignKey(x => x.SectionId);

            builder.HasData(new Section
            {
                Id = Guid.NewGuid(),
                Name = "A"
            },
            new Section
            {
                Id = Guid.NewGuid(),
                Name = "B"
            },
            new Section
            {
                Id = Guid.NewGuid(),
                Name = "C"
            });
        }
    }
}
