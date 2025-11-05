using SmartCourses.DAL.Common.Entities;

namespace SmartCourses.DAL.Entities
{
    public class Category : BaseAuditableEntity<int>
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? IconPath { get; set; }

        // Navigation Properties
        public virtual ICollection<Course> Courses { get; set; } = new HashSet<Course>();

    }
}
