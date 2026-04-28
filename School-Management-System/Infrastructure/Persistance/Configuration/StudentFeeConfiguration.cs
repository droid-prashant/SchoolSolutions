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
    public class StudentFeeConfiguration : IEntityTypeConfiguration<StudentFee>
    {
        public void Configure(EntityTypeBuilder<StudentFee> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasMany(x => x.Payments)
                   .WithOne(x => x.StudentFee)
                   .HasForeignKey(x => x.StudentFeeId);
            builder.HasMany(x => x.FeeAdjustments)
                   .WithOne(x => x.StudentFee)
                   .HasForeignKey(x => x.StudentFeeId);
        }
    }
}
