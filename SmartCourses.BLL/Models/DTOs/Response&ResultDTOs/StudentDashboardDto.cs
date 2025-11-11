using SmartCourses.BLL.Models.DTOs.CourseDTOs;
using SmartCourses.BLL.Models.DTOs.Enrollment_ReviewDTOs;

namespace SmartCourses.BLL.Models.DTOs.Response_ResultDTOs
{
    public class StudentDashboardDto
    {
        // Enrollment statistics
        public int EnrolledCoursesCount { get; set; }
        public int CompletedCoursesCount { get; set; }
        public int InProgressCoursesCount { get; set; }

        // Progress
        public decimal OverallProgress { get; set; }

        // Lists
        public List<EnrollmentDto> RecentEnrollments { get; set; } = new();
        public List<EnrollmentDto> InProgressCourses { get; set; } = new();
        public List<CourseListDto> RecommendedCourses { get; set; } = new();
        public List<CourseListDto> CompletedCourses { get; set; } = new();

        // Achievements (optional)
        public int TotalCertificates { get; set; }
        public int TotalWatchedHours { get; set; }

    }
}
