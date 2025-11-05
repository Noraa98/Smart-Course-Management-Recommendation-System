using SmartCourses.DAL.Common.Entities;
using SmartCourses.DAL.Common.Enums;
using SmartCourses.DAL.Entities.Identity;
using SmartCourses.DAL.Entities.RelationshipsTables;

namespace SmartCourses.DAL.Entities
{
    public class Course : BaseAuditableEntity<int>
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public string? ThumbnailPath { get; set; }
        public CourseLevel Level { get; set; }
        public bool IsPublished { get; set; } = false;
        public decimal? Price { get; set; }
        public int DurationInHours { get; set; }

        // Foreign Keys
        public int CategoryId { get; set; }
        public string InstructorId { get; set; } = string.Empty;

        // Navigation Properties
        public virtual Category Category { get; set; } = null!;
        public virtual ApplicationUser Instructor { get; set; } = null!;
        public virtual ICollection<Section> Sections { get; set; } = new HashSet<Section>();
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new HashSet<Enrollment>();
        public virtual ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
        public virtual ICollection<CourseSkill> CourseSkills { get; set; } = new HashSet<CourseSkill>();

    }
}
