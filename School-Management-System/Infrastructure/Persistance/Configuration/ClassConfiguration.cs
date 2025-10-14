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
    public class ClassConfiguration : IEntityTypeConfiguration<ClassRoom>
    {
        public void Configure(EntityTypeBuilder<ClassRoom> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasMany(x => x.ClassSections)
                   .WithOne(x => x.ClassRoom)
                   .HasForeignKey(x => x.ClassId);

            builder.HasMany(x => x.ClassCourses)
                   .WithOne(x => x.ClassRoom)
                   .HasForeignKey(x => x.ClassRoomId);


            builder.HasData(
                    new ClassRoom
                    {
                        Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                        Name = "One",
                        RoomNumber = "1",
                        AcademicYear = "2024/2025"
                    },
                    new ClassRoom
                    {
                        Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                        Name = "Two",
                        RoomNumber = "2",
                        AcademicYear = "2024/2025"
                    },
                    new ClassRoom
                    {
                        Id = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                        Name = "Three",
                        RoomNumber = "3",
                        AcademicYear = "2024/2025"
                    },
                    new ClassRoom
                    {
                        Id = Guid.Parse("10000000-0000-0000-0000-000000000004"),
                        Name = "Four",
                        RoomNumber = "4",
                        AcademicYear = "2024/2025"
                    },
                    new ClassRoom
                    {
                        Id = Guid.Parse("10000000-0000-0000-0000-000000000005"),
                        Name = "Five",
                        RoomNumber = "5",
                        AcademicYear = "2024/2025"
                    },
                    new ClassRoom
                    {
                        Id = Guid.Parse("10000000-0000-0000-0000-000000000006"),
                        Name = "Six",
                        RoomNumber = "6",
                        AcademicYear = "2024/2025"
                    },
                    new ClassRoom
                    {
                        Id = Guid.Parse("10000000-0000-0000-0000-000000000007"),
                        Name = "Seven",
                        RoomNumber = "7",
                        AcademicYear = "2024/2025"
                    },
                    new ClassRoom
                    {
                        Id = Guid.Parse("10000000-0000-0000-0000-000000000008"),
                        Name = "Eight",
                        RoomNumber = "8",
                        AcademicYear = "2024/2025"
                    },
                    new ClassRoom
                    {
                        Id = Guid.Parse("10000000-0000-0000-0000-000000000009"),
                        Name = "Nine",
                        RoomNumber = "9",
                        AcademicYear = "2024/2025"
                    },
                    new ClassRoom
                    {
                        Id = Guid.Parse("10000000-0000-0000-0000-000000000010"),
                        Name = "Ten",
                        RoomNumber = "10",
                        AcademicYear = "2024/2025"
                    }
             );
        }
    }
}
