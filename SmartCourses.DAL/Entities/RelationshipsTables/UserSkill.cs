using SmartCourses.DAL.Entities.Identity;

namespace SmartCourses.DAL.Entities.RelationshipsTables
{
    public class UserSkill
    {
        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser User { get; set; } = null!;

        public int SkillId { get; set; }
        public virtual Skill Skill { get; set; } = null!;

        public int ProficiencyLevel { get; set; } = 1; // 1-5
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    }
}
