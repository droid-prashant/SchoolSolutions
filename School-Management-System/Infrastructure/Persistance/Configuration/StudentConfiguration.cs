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
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.HasMany(x => x.StudentEnrollments)
                   .WithOne(x => x.Student)
                   .HasForeignKey(x => x.StudentId);

            builder.HasOne(x => x.Province)
                   .WithMany()
                   .HasForeignKey(x => x.ProvinceId);

            builder.HasOne(x => x.District)
                   .WithMany()
                   .HasForeignKey(x => x.DistrictId);

            builder.HasOne(x => x.Municipality)
                   .WithMany()
                   .HasForeignKey(x => x.MunicipalityId);
        }
    }
}
