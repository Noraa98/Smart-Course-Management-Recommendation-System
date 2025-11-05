namespace SmartCourses.DAL.Entities.RelationshipsTables
{
    public class CourseSkill
    {
        public int CourseId { get; set; }
        public virtual Course Course { get; set; } = null!;

        public int SkillId { get; set; }
        public virtual Skill Skill { get; set; } = null!;

    }
}
