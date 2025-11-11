using Microsoft.Extensions.DependencyInjection;
using SmartCourses.BLL.Services.Contracts;
using SmartCourses.BLL.Services.Implementations;
using SmartCourses.BLL.Services.Implementations.AuthImplmentation;
using SmartCourses.BLL.Services.Interfaces;
using SmartCourses.BLL.Services.Interfaces.Auth;
using System.Reflection;

namespace SmartCourses.BLL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBLLServices(this IServiceCollection services)
        {
            // 1. Register AutoMapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());



            //// 2. Register Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IEnrollmentService, EnrollmentService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ISkillService, SkillService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IFileService, FileService>();

            // 3. Register HttpContextAccessor (for getting current user)
            services.AddHttpContextAccessor();

            return services;
        }
    }
}
