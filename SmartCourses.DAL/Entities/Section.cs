using SmartCourses.DAL.Common.Entities;

namespace SmartCourses.DAL.Entities
{
    public class Section : BaseAuditableEntity<int>
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Order { get; set; }

        // Foreign Keys
        public int CourseId { get; set; }

        // Navigation Properties
        public virtual Course Course { get; set; } = null!;
        public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

    }
}
