using SmartCourses.BLL.Models.DTOs.CourseDTOs;
using SmartCourses.BLL.Models.DTOs.Enrollment_ReviewDTOs;

namespace SmartCourses.BLL.Models.DTOs.Response_ResultDTOs
{
    public class InstructorDashboardDto
    {
        public int TotalCourses { get; set; }
        public int PublishedCourses { get; set; }
        public int TotalEnrollments { get; set; }
        public int TotalReviews { get; set; }
        public double AverageRating { get; set; }

        public List<CourseListDto> MyCourses { get; set; } = new();
        public List<ReviewDto> RecentReviews { get; set; } = new();
    }
}
