using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartCourses.DAL.Common.Enums;
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
            var options = new JsonSerializerOptions {
                PropertyNameCaseInsensitive = true,
               Converters = { new JsonStringEnumConverter() }
            };

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
                    Console.WriteLine($" Successfully seeded {users.Count} users.");
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
                Console.WriteLine($" Successfully seeded {categories.Count} categories.");
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
                Console.WriteLine($" Successfully seeded {skills.Count} skills.");

            }
        }

        private void SeedCourses(JsonSerializerOptions options)
        {
            if (!_context.Courses.Any())
            {
                var path = Path.Combine(AppContext.BaseDirectory, "Persistence", "Data", "Seeds", "courses.json");

                if (!File.Exists(path))
                {
                    Console.WriteLine($"❌ Error: courses.json file not found at {path}");
                    return;
                }

                var data = File.ReadAllText(path);
                var coursesData = JsonSerializer.Deserialize<List<CourseJsonModel>>(data, options);

                if (coursesData?.Count > 0)
                {
                    var instructors = _userManager.Users
                        .Where(u => u.Email != null &&
                               (u.Email.Contains("linda.smith") || u.Email.Contains("mohamed.ali")))
                        .ToList();

                    if (instructors.Count < 2)
                    {
                        Console.WriteLine($"❌ Error: Not enough instructors found. Expected 2, found {instructors.Count}");
                        return;
                    }

                    var lindaId = instructors.First(u => u.Email!.Contains("linda.smith")).Id;
                    var mohamedId = instructors.First(u => u.Email!.Contains("mohamed.ali")).Id;

                    var categories = _context.Categories.ToList();
                    var categoryMap = categories.ToDictionary(c => c.Name, c => c.Id);

                    foreach (var courseData in coursesData)
                    {
                        var actualInstructorId = courseData.InstructorId == 1 ? lindaId : mohamedId;

                        if (!categoryMap.TryGetValue(courseData.CategoryName, out int categoryId))
                        {
                            Console.WriteLine($"Warning: Category '{courseData.CategoryName}' not found for course '{courseData.Title}'. Skipping.");
                            continue;
                        }

                        var course = new Course
                        {
                            Title = courseData.Title,
                            ShortDescription = courseData.ShortDescription,
                            Description = courseData.Description,
                            Level = courseData.Level,
                            IsPublished = courseData.IsPublished,
                            Price = courseData.Price,
                            DurationInHours = courseData.DurationInHours,
                            CategoryId = categoryId,
                            InstructorId = actualInstructorId,
                            CreatedBy = courseData.CreatedBy,
                            LastModifiedBy = courseData.LastModifiedBy,
                            CreatedOn = DateTime.UtcNow,
                            LastModifiedOn = DateTime.UtcNow
                        };

                        _context.Courses.Add(course);
                    }

                    try
                    {
                        _context.SaveChanges();
                        Console.WriteLine($"✅ Successfully seeded {coursesData.Count} courses.");

                        // Seed realistic sections and lessons with local video paths if none exist
                        if (!_context.Sections.Any() && !_context.Lessons.Any())
                        {
                            var courses = _context.Courses.ToList();
                            foreach (var c in courses)
                            {
                                // Create two sections per course
                                for (int s = 1; s <= 2; s++)
                                {
                                    var section = new Section
                                    {
                                        Title = s == 1 ? "Getting Started" : "Core Concepts",
                                        Description = s == 1 ? "Introduction and setup" : "Deep dive into key topics",
                                        Order = s,
                                        CourseId = c.Id,
                                        CreatedBy = "System",
                                        LastModifiedBy = "System",
                                        CreatedOn = DateTime.UtcNow,
                                        LastModifiedOn = DateTime.UtcNow
                                    };
                                    _context.Sections.Add(section);
                                    _context.SaveChanges();

                                    // Create three lessons per section with local video paths
                                    for (int l = 1; l <= 3; l++)
                                    {
                                        var minutes = 5 + (l * 3);
                                        var lesson = new Lesson
                                        {
                                            Title = $"Lesson {s}.{l}",
                                            Description = l == 1 ? "Overview and context" : (l == 2 ? "Hands-on example" : "Summary and next steps"),
                                            ContentType = ContentType.Video,
                                            ContentPath = $"/videos/course{c.Id}_s{s}_l{l}.mp4", // Assume placeholder MP4 files exist
                                            DurationInMinutes = minutes,
                                            Order = l,
                                            IsFree = s == 1 && l == 1, // First lesson free
                                            SectionId = section.Id,
                                            CreatedBy = "System",
                                            LastModifiedBy = "System",
                                            CreatedOn = DateTime.UtcNow,
                                            LastModifiedOn = DateTime.UtcNow
                                        };
                                        _context.Lessons.Add(lesson);
                                    }
                                }
                            }
                            _context.SaveChanges();
                            Console.WriteLine("✅ Seeded Sections and video Lessons with /videos paths for each course.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Error saving courses: {ex.Message}");
                        if (ex.InnerException != null)
                            Console.WriteLine($"   Inner Exception: {ex.InnerException.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("⚠️ Warning: No course data found in courses.json");
                }
            }
            else
            {
                Console.WriteLine("ℹ️ Courses already exist. Skipping seed.");
            }
        }

        // Helper class for deserializing JSON (add this inside DbInitializer class)
        private class CourseJsonModel
        {
            public string Title { get; set; } = null!;
            public string ShortDescription { get; set; } = null!;
            public string Description { get; set; } = null!;
            public CourseLevel Level { get; set; }
            public bool IsPublished { get; set; }
            public decimal? Price { get; set; }
            public int DurationInHours { get; set; }
            public string CategoryName { get; set; } = null!;  // Changed from CategoryId
            public int InstructorId { get; set; }  // Temporary, will be mapped to GUID
            public string CreatedBy { get; set; } = "System";
            public string LastModifiedBy { get; set; } = "System";
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