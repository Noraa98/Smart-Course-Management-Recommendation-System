using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartCourses.DAL.Common.Enums;
using SmartCourses.DAL.Common.JsonConverters;
using SmartCourses.DAL.Contracts;
using SmartCourses.DAL.Entities;
using SmartCourses.DAL.Entities.Identity;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartCourses.DAL.Persistence.Data.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            if (_context.Database.GetPendingMigrations().Any())
            {
                _context.Database.Migrate();
            }
        }

        public void Seed()
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            SeedRoles(options);
            SeedUsers(options);
            SeedCategories(options);
            SeedSkills(options);
            SeedCourses(options);
        }

        private void SeedRoles(JsonSerializerOptions options)
        {
            if (!_roleManager.Roles.Any())
            {
                var path = Path.Combine(AppContext.BaseDirectory, "Persistence", "Data", "Seeds", "roles.json");
                var data = File.ReadAllText(path);
                var roles = JsonSerializer.Deserialize<List<IdentityRole>>(data, options);

                if (roles?.Count > 0)
                {
                    foreach (var role in roles)
                        _roleManager.CreateAsync(new IdentityRole(role.Name!)).Wait();
                }
            }
        }

        private void SeedUsers(JsonSerializerOptions options)
        {
            if (!_userManager.Users.Any())
            {
                var path = Path.Combine(AppContext.BaseDirectory, "Persistence", "Data", "Seeds", "users.json");
                var data = File.ReadAllText(path);
                var users = JsonSerializer.Deserialize<List<JsonUser>>(data, options);

                if (users?.Count > 0)
                {
                    foreach (var u in users)
                    {
                        var user = new ApplicationUser
                        {
                            UserName = u.UserName,
                            Email = u.Email,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            EmailConfirmed = true,
                            CreatedAt = DateTime.UtcNow
                        };
                        _userManager.CreateAsync(user, "Password@123").Wait();
                        _userManager.AddToRoleAsync(user, u.Role).Wait();
                    }
                }
            }
        }

        private void SeedCategories(JsonSerializerOptions options)
        {
            if (!_context.Categories.Any())
            {
                var path = Path.Combine(AppContext.BaseDirectory, "Persistence", "Data", "Seeds", "categories.json");
                var data = File.ReadAllText(path);
                var categories = JsonSerializer.Deserialize<List<Category>>(data, options);
                if (categories?.Count > 0)
                {
                    foreach (var category in categories)
                    {
                        category.CreatedOn = DateTime.UtcNow;
                        category.LastModifiedOn = DateTime.UtcNow;
                    }
                    _context.Categories.AddRange(categories);
                    _context.SaveChanges();
                }
            }
        }

        private void SeedSkills(JsonSerializerOptions options)
        {
            if (!_context.Skills.Any())
            {
                var path = Path.Combine(AppContext.BaseDirectory, "Persistence", "Data", "Seeds", "skills.json");
                var data = File.ReadAllText(path);
                var skills = JsonSerializer.Deserialize<List<Skill>>(data, options);
                if (skills?.Count > 0)
                {
                    foreach (var skill in skills)
                    {
                        skill.CreatedOn = DateTime.UtcNow;
                        skill.LastModifiedOn = DateTime.UtcNow;
                    }
                    _context.Skills.AddRange(skills);
                    _context.SaveChanges();
                }
            }
        }

        private void SeedCourses(JsonSerializerOptions options)
        {
            if (!_context.Courses.Any())
            {
                var path = Path.Combine(AppContext.BaseDirectory, "Persistence", "Data", "Seeds", "courses.json");
                var data = File.ReadAllText(path);

                if (!options.Converters.OfType<JsonStringEnumConverter>().Any())
                    options.Converters.Add(new JsonStringEnumConverter());

                if (!options.Converters.OfType<NumberToStringConverter>().Any())
                    options.Converters.Add(new NumberToStringConverter());

                var courses = JsonSerializer.Deserialize<List<Course>>(data, options);

                if (courses?.Count > 0)
                {
                    foreach (var course in courses)
                    {
                        //  InstructorId
                        var instructorExists = _userManager.Users.Any(u => u.Id == course.InstructorId);
                        if (!instructorExists)
                        {
                            Console.WriteLine($"Error: InstructorId '{course.InstructorId}' does not exist for course '{course.Title}'");
                            continue; 
                        }

                        //  CategoryId
                        var categoryExists = _context.Categories.Any(c => c.Id == course.CategoryId);
                        if (!categoryExists)
                        {
                            Console.WriteLine($"Error: CategoryId '{course.CategoryId}' does not exist for course '{course.Title}'");
                            continue;
                        }

                        course.CreatedOn = DateTime.UtcNow;
                        course.LastModifiedOn = DateTime.UtcNow;

                        _context.Courses.Add(course);
                    }

                    try
                    {
                        _context.SaveChanges();
                        Console.WriteLine("Courses seeded successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error saving courses: " + ex.Message);
                    }
                }
            }
        }


        // helper class for JSON user structure
        private class JsonUser
        {
            public string UserName { get; set; } = null!;
            public string Email { get; set; } = null!;
            public string FirstName { get; set; } = null!;
            public string LastName { get; set; } = null!;
            public string Role { get; set; } = null!;
        }
    }
}