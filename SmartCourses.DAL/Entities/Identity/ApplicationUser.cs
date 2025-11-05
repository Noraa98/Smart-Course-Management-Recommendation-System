using Microsoft.AspNetCore.Identity;
using SmartCourses.DAL.Entities.RelationshipsTables;

namespace SmartCourses.DAL.Entities.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? ProfilePicturePath { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }


        // Navigation Properties
        public virtual ICollection<Course> CoursesInstructed { get; set; } = new HashSet<Course>();
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new HashSet<Enrollment>();
        public virtual ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
        public virtual ICollection<UserSkill> UserSkills { get; set; } = new HashSet<UserSkill>();
        public virtual ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();
    }
}
