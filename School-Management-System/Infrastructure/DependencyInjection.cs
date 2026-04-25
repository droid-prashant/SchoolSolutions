using System.Diagnostics;
using Application.Academic.Interfaces;
using Application.ClassSections.Interfaces;
using Application.Common.Interfaces;
using Application.Courses.Interfaces;
using Application.Dashboard.Interfaces;
using Application.Fees.Interfaces;
using Application.Identity.Interfaces;
using Application.Master.Interface;
using Application.Students.Interfaces;
using Application.SubjectMarks.Interfaces;
using Application.Teachers;
using Infrastructure.Identity;
using Infrastructure.Persistance;
using Infrastructure.Services.Academics;
using Infrastructure.Services.ClassSections;
using Infrastructure.Services.Courses;
using Infrastructure.Services.Dashboard;
using Infrastructure.Services.Fees;
using Infrastructure.Services.MasterData;
using Infrastructure.Services.Students;
using Infrastructure.Services.SubjectMarks;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
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

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddScoped<RoleManager<ApplicationRole>>();
            services.AddScoped<UserManager<ApplicationUser>>();
            services.AddScoped<IIdentityService, IdentityService>();

            services.AddScoped<IDashboardService, DasboardService>();
            services.AddScoped<IAcademicService, AcademicService>();
            services.AddScoped<IMasterDataService, MasterDataService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IClassSectionService, ClassSectionService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IExamService, ExamService>();
            services.AddScoped<IFeeService, FeeService>();
            services.AddScoped<ITeacherService, TeacherService>();
            return services;
        }
    }
}
