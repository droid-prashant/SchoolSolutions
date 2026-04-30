using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>, IApplicationDbContext

    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<School> Schools => Set<School>();
        public DbSet<Teacher> Teachers => Set<Teacher>();
        public DbSet<TeacherClassSection> TeacherClassSections => Set<TeacherClassSection>();
        public DbSet<TeacherQualification> TeacherQualifications => Set<TeacherQualification>();
        public DbSet<TeacherExperience> TeacherExperiences => Set<TeacherExperience>();
        public DbSet<TeacherDocument> TeacherDocuments => Set<TeacherDocument>();
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

        public DbSet<AcademicYear> AcademicYears => Set<AcademicYear>();

        public DbSet<StudentEnrollment> StudentEnrollments => Set<StudentEnrollment>();

        public DbSet<Province> Provinces => Set<Province>();

        public DbSet<District> Districts => Set<District>();

        public DbSet<Municipality> Municipalities => Set<Municipality>();

        public DbSet<StudentCharacterCertificateLog> studentCharacterCertificateLogs => Set<StudentCharacterCertificateLog>();

        public DbSet<StudentTransferCertificateLog> studentTransferCertificateLogs => Set<StudentTransferCertificateLog>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<AuditableEntry>())
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
