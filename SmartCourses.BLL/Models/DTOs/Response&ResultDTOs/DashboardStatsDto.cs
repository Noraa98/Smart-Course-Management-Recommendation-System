using SmartCourses.BLL.Models.DTOs.CourseDTOs;
using SmartCourses.BLL.Models.DTOs.Enrollment_ReviewDTOs;

namespace SmartCourses.BLL.Models.DTOs.Response_ResultDTOs
{
    public class DashboardStatsDto
    {
        public int TotalUsers { get; set; }
        public int TotalCourses { get; set; }
        public int TotalEnrollments { get; set; }
        public int TotalCategories { get; set; }

        public int StudentsCount { get; set; }
        public int InstructorsCount { get; set; }

        public List<CourseListDto> RecentCourses { get; set; } = new();
        public List<CourseListDto> TopRatedCourses { get; set; } = new();
        public List<EnrollmentDto> RecentEnrollments { get; set; } = new();
    }
}
