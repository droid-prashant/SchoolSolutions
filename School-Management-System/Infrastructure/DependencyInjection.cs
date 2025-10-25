using System.Diagnostics;
using Application.ClassSections.Interfaces;
using Application.Common.Interfaces;
using Application.Courses.Interfaces;
using Application.Fees.Interfaces;
using Application.Students.Interfaces;
using Application.SubjectMarks.Interfaces;
using Infrastructure.Persistance;
using Infrastructure.Services.ClassSections;
using Infrastructure.Services.Courses;
using Infrastructure.Services.Fees;
using Infrastructure.Services.Students;
using Infrastructure.Services.SubjectMarks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(option =>
            {
                option.UseNpgsql(configuration.GetConnectionString("DbString"));
            });
            services.AddScoped<IApplicationDbContext>(x=>x.GetRequiredService<ApplicationDbContext>());
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IClassSectionService, ClassSectionService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IExamService, ExamService>();
            services.AddScoped<IFeeService, FeeService>();
            return services;
        }
    }
}
