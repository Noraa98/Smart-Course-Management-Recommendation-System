using SmartCourses.BLL.Models.DTOs.CourseDTOs;
using SmartCourses.BLL.Models.DTOs.Enrollment_ReviewDTOs;

namespace SmartCourses.BLL.Models.DTOs.Response_ResultDTOs
{
    public class InstructorDashboardDto
    {
        // Course statistics
        public int TotalCourses { get; set; }
        public int PublishedCourses { get; set; }
        public int UnpublishedCourses { get; set; }

        // Enrollment statistics
        public int TotalEnrollments { get; set; }
        public int ActiveEnrollments { get; set; }
        public int CompletedEnrollments { get; set; }

        // Review statistics
        public int TotalReviews { get; set; }
        public double AverageRating { get; set; }

        // Revenue
        public decimal TotalRevenue { get; set; }

        // Lists
        public List<CourseListDto> MyCourses { get; set; } = new();
        public List<ReviewDto> RecentReviews { get; set; } = new();
        public List<EnrollmentDto> RecentEnrollments { get; set; } = new();

    }
}
