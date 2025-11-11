namespace SmartCourses.BLL.Models.DTOs.CourseDTOs
{
    public class CourseListDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public string? ThumbnailPath { get; set; }
        public string Level { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public int DurationInHours { get; set; }
        public bool IsPublished { get; set; }

        public string CategoryName { get; set; } = string.Empty;
        public string InstructorName { get; set; } = string.Empty;

        public int EnrollmentCount { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }

        public List<string> SkillNames { get; set; } = new();
    }
}
