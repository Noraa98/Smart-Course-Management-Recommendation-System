using SmartCourses.BLL.Models.DTOs.CourseDTOs;
using SmartCourses.BLL.Models.DTOs.Enrollment_ReviewDTOs;

namespace SmartCourses.BLL.Models.DTOs.Response_ResultDTOs
{
    public class DashboardStatsDto
    {
        // Total counts
        public int TotalUsers { get; set; }
        public int TotalCourses { get; set; }
        public int TotalEnrollments { get; set; }
        public int TotalCategories { get; set; }

        // Role-specific counts
        public int StudentsCount { get; set; }
        public int InstructorsCount { get; set; }
        public int AdminsCount { get; set; }

        // Recent data
        public List<CourseListDto> RecentCourses { get; set; } = new();
        public List<CourseListDto> TopRatedCourses { get; set; } = new();
        public List<EnrollmentDto> RecentEnrollments { get; set; } = new();

        // Statistics
        public decimal TotalRevenue { get; set; }
        public int PublishedCoursesCount { get; set; }
        public int UnpublishedCoursesCount { get; set; }
    }
}
