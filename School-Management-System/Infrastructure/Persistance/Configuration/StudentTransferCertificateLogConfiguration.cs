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
    public class StudentTransferCertificateLogConfiguration : IEntityTypeConfiguration<StudentTransferCertificateLog>
    {
        public void Configure(EntityTypeBuilder<StudentTransferCertificateLog> builder)
        {
            builder.HasOne(x => x.Student)
                   .WithMany()
                   .HasForeignKey(x => x.StudentId);
        }
    }
}
