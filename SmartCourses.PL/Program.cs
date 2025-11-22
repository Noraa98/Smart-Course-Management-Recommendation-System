using Microsoft.AspNetCore.Identity;
using Serilog;
using SmartCourses.BLL;
using SmartCourses.DAL.Contracts;
using SmartCourses.DAL.Entities.Identity;
using SmartCourses.DAL.Persistence;
using SmartCourses.DAL.Persistence.Data;
using SmartCourses.DAL.Persistence.Data.DbInitializer;
using System.Text.Json.Serialization;

namespace SmartCourses.PL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .WriteTo.Console()
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            builder.Host.UseSerilog();



            // Add services to the container.
            builder.Services.AddControllersWithViews()
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;

                        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    });


            // Register DAL Services
            builder.Services.AddDALServices(builder.Configuration);


            // BLL Services (AutoMapper, Business Services)
            builder.Services.AddBLLServices();


            // Session (Optional - for shopping cart, etc.)
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });


            // Configure Cookie Settings
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.SlidingExpiration = true;
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Lax;
            });


            //  Register Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = false;
            })
             .AddEntityFrameworkStores<ApplicationDbContext>()
             .AddDefaultTokenProviders();

            // Register DbInitializer
            builder.Services.AddScoped<IDbInitializer, DbInitializer>();

            var app = builder.Build();


            // Seed Database

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var dbInitializer = services.GetRequiredService<IDbInitializer>();
                     dbInitializer.Initialize();
                    Log.Information("Database initialized successfully");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "An error occurred while initializing the database");
                }
            }

            // Initialize and seed the database
            using (var scope = app.Services.CreateScope())
            {
                var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
                dbInitializer.Initialize();
                dbInitializer.Seed();
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            // Serve default and static files from wwwroot (images, videos, css, js, svg, etc.)
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            // Authentication must come before Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            // Map Areas route before default to ensure area URLs resolve
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}")
                .WithStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();


            // Run Application
            try
            {
                Log.Information("Starting Smart Courses Application");
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
