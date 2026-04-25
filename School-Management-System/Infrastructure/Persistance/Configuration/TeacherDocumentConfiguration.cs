using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.Configuration
{
    public class TeacherDocumentConfiguration : IEntityTypeConfiguration<TeacherDocument>
    {
        public void Configure(EntityTypeBuilder<TeacherDocument> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.DocumentType).IsRequired().HasMaxLength(80);
            builder.Property(x => x.DocumentTitle).IsRequired().HasMaxLength(150);
            builder.Property(x => x.FilePath).IsRequired().HasMaxLength(500);
            builder.Property(x => x.OriginalFileName).IsRequired().HasMaxLength(255);
            builder.Property(x => x.MimeType).IsRequired().HasMaxLength(100);
            builder.HasIndex(x => new { x.TeacherId, x.DocumentType, x.IsDeleted });
        }
    }
}
