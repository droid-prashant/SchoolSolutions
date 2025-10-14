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
    public class TeacherClassSectionConfiguration : IEntityTypeConfiguration<TeacherClassSection>
    {
        public void Configure(EntityTypeBuilder<TeacherClassSection> builder)
        {
            
        }
    }
}
