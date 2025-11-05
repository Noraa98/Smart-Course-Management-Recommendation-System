using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartCourses.DAL.Contracts;
using SmartCourses.DAL.Entities.Identity;
using SmartCourses.DAL.Persistence.Data.DbInitializer;
using SmartCourses.DAL.Persistence.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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

            

            // ✅ 3. Register DbInitializer
            services.AddScoped<IDbInitializer, DbInitializer>();

            // ✅ 4. Register Unit of Work (سنضيفه لاحقاً)
            // services.AddScoped<IUnitOfWork, UnitOfWork>();

            // ✅ 5. Register Repositories (سنضيفهم لاحقاً)
            // services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            return services;
        }
    }
}
