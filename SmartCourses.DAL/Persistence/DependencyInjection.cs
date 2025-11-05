using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartCourses.DAL.Contracts;
using SmartCourses.DAL.Contracts.Repositories;
using SmartCourses.DAL.Persistence.Data;
using SmartCourses.DAL.Persistence.Data.DbInitializer;
using SmartCourses.DAL.Persistence.Repositories;
using SmartCourses.DAL.Persistence.UnitOfWork;

namespace SmartCourses.DAL.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDALServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // 1. Register DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));


            });

            // 3. Register DbInitializer
            services.AddScoped<IDbInitializer, DbInitializer>();

            // 4. Register Unit of Work 
             services.AddScoped<IUnitOfWork, Persistence.UnitOfWork.UnitOfWork>();

            // 5.Register Generic Repository
            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));

            // 6. Register Specialized Repositories
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ISkillRepository, SkillRepository>();
            services.AddScoped<ILessonProgressRepository, LessonProgressRepository>();

            return services;
        }
    }
}
