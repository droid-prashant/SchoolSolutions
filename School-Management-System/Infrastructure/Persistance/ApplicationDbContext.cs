using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance
{
    public class ApplicationDbContext:DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options) { }
        public DbSet<Teacher> Teachers => Set<Teacher>();
        public DbSet<Student> Students => Set<Student>();
        public DbSet<Course> Courses => Set<Course>();

        public DbSet<ClassRoom> ClassRooms => Set<ClassRoom>();
        public DbSet<Section> Sections => Set<Section>();
        public DbSet<ClassSection> ClassSections => Set<ClassSection>();
        public DbSet<ClassCourse> ClassCourses => Set<ClassCourse>();

        public DbSet<SubjectMark> SubjectMarks => Set<SubjectMark>();

        public DbSet<ExamResult> ExamResults => Set<ExamResult>();
        public DbSet<FeeType> FeeTypes => Set<FeeType>();
        public DbSet<FeeStructure> FeeStructures => Set<FeeStructure>();
        public DbSet<StudentFee> StudentFees => Set<StudentFee>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<FeeAdjustment> FeeAdjustments => Set<FeeAdjustment>();
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach(var entry in ChangeTracker.Entries<AuditableEntry>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.ModifiedDate = DateTime.UtcNow;
                        break;
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all configurations from the assembly (including ClassConfiguration)
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
