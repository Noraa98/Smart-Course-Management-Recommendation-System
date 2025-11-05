using SmartCourses.DAL.Common.Entities;
using SmartCourses.DAL.Entities.Identity;

namespace SmartCourses.DAL.Entities
{
    public class Review : BaseAuditableEntity<int>
    {
        public int Rating { get; set; }
        public string? Comment { get; set; }

        // Foreign Keys
        public string UserId { get; set; } = string.Empty;
        public int CourseId { get; set; }

        // Navigation Properties
        public virtual ApplicationUser User { get; set; } = null!;
        public virtual Course Course { get; set; } = null!;

    }
}
