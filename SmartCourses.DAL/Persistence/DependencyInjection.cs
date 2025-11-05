using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartCourses.DAL.Contracts;
using SmartCourses.DAL.Persistence.Data;
using SmartCourses.DAL.Persistence.Data.DbInitializer;
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
             services.AddScoped<IUnitOfWork, UnitOfWorks>();

            // 5. Register Repositories 
            // services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            return services;
        }
    }
}
