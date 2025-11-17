using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartCourses.DAL.Common.Entities;
using SmartCourses.DAL.Entities;
using SmartCourses.DAL.Entities.Identity;
using SmartCourses.DAL.Entities.RelationshipsTables;
using System.Reflection;

namespace SmartCourses.DAL.Persistence.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        // to hold the current user ID for audit purposes
        private readonly string _currentUserId;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
            _currentUserId = "System"; // Default value
        }

        // Overloaded constructor to accept current user ID
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,string currentUserId) : base(options)
        {
            _currentUserId = currentUserId ?? "System";
        }

   
        // DbSets - Main Entities
        public DbSet<Category> Categories { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<LessonProgress> LessonProgresses { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Notification> Notifications { get; set; }

    
        // DbSets - Junction Tables
        public DbSet<CourseSkill> CourseSkills { get; set; }
        public DbSet<UserSkill> UserSkills { get; set; }

       
        // OnModelCreating - Apply Configurations
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all configurations from assembly automatically
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Customize Identity table names (Optional but recommended)
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("Users");
            });

            modelBuilder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable("Roles");
            });

            modelBuilder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UserRoles");
            });

            modelBuilder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaims");
            });

            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogins");
            });

            modelBuilder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("RoleClaims");
            });

            modelBuilder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserTokens");
            });
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            
            optionsBuilder.UseSqlServer(
                options => options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
        }

        // SaveChangesAsync - Override for Audit Trail
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Handle Audit Trail automatically
            HandleAuditableEntities();

            return await base.SaveChangesAsync(cancellationToken);
        }

      
        // SaveChanges - Override for Audit Trail
        public override int SaveChanges()
        {
            // Handle Audit Trail automatically
            HandleAuditableEntities();

            return base.SaveChanges();
        }

        // Private Methods
        private void HandleAuditableEntities()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseAuditableEntity<int> &&
                           (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (BaseAuditableEntity<int>)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedBy = _currentUserId;
                    entity.CreatedOn = DateTime.UtcNow;
                    entity.LastModifiedBy = _currentUserId;
                    entity.LastModifiedOn = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entity.LastModifiedBy = _currentUserId;
                    entity.LastModifiedOn = DateTime.UtcNow;

                    // Prevent modification of CreatedBy and CreatedOn on updates
                    entry.Property(nameof(entity.CreatedBy)).IsModified = false;
                    entry.Property(nameof(entity.CreatedOn)).IsModified = false;
                }
            }
        }
    }
}