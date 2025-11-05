using SmartCourses.DAL.Common.Entities;
using SmartCourses.DAL.Entities.RelationshipsTables;

namespace SmartCourses.DAL.Entities
{
    public class Skill : BaseAuditableEntity<int>
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        // Navigation Properties
        public virtual ICollection<CourseSkill> CourseSkills { get; set; } = new HashSet<CourseSkill>();
        public virtual ICollection<UserSkill> UserSkills { get; set; } = new HashSet<UserSkill>();

    }
}
