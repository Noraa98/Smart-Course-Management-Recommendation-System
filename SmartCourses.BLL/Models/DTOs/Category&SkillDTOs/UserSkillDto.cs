namespace SmartCourses.BLL.Models.DTOs
{
    public class UserSkillDto
    {
        public string UserId { get; set; } = string.Empty;
        public int SkillId { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public int ProficiencyLevel { get; set; } = 1; // 1-5
        public DateTime AddedAt { get; set; }

    }
}
