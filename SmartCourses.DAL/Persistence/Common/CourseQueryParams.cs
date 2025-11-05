using SmartCourses.DAL.Common.Enums;

namespace SmartCourses.DAL.Persistence.Common
{
    public class CourseQueryParams : QueryParams
    {
        public int? CategoryId { get; set; }
        public CourseLevel? Level { get; set; }
        public int? SkillId { get; set; }
        public bool? IsPublished { get; set; }
        public string? InstructorId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinDuration { get; set; }
        public int? MaxDuration { get; set; }
    }
}
