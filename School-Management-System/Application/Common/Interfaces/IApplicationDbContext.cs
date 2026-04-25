using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<AcademicYear> AcademicYears { get; }
        DbSet<StudentEnrollment> StudentEnrollments { get; }
        DbSet<Teacher> Teachers { get; }
        DbSet<TeacherClassSection> TeacherClassSections { get; }
        DbSet<TeacherQualification> TeacherQualifications { get; }
        DbSet<TeacherExperience> TeacherExperiences { get; }
        DbSet<TeacherDocument> TeacherDocuments { get; }
        DbSet<Student> Students { get; }
        DbSet<Course> Courses { get; }
        DbSet<ClassRoom> ClassRooms { get; }
        DbSet<Section> Sections { get; }
        DbSet<ClassSection> ClassSections { get; }
        DbSet<ClassCourse> ClassCourses { get; }
        DbSet<SubjectMark> SubjectMarks { get; }
        DbSet<ExamResult> ExamResults { get; }
        DbSet<FeeType> FeeTypes { get; }
        DbSet<FeeStructure> FeeStructures { get; }
        DbSet<StudentFee> StudentFees { get; }
        DbSet<Payment> Payments { get; }
        DbSet<FeeAdjustment> FeeAdjustments { get; }
        DbSet<Province> Provinces { get; }
        DbSet<District> Districts { get; }
        DbSet<Municipality> Municipalities { get; }
        DbSet<StudentCharacterCertificateLog> studentCharacterCertificateLogs { get; }
        DbSet<StudentTransferCertificateLog> studentTransferCertificateLogs { get; }
        DatabaseFacade Database { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
