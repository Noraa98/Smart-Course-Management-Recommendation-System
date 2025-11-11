using SmartCourses.BLL.Models.DTOs.BaseDTOs;

namespace SmartCourses.BLL.Models.DTOs.Enrollment_ReviewDTOs
{
    public class EnrollmentDto : BaseAuditableDto<int>
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;

        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string? CourseThumbnail { get; set; }

        public DateTime EnrolledAt { get; set; }
        public decimal ProgressPercent { get; set; }
        public DateTime? CompletedAt { get; set; }
        public bool IsCompleted { get; set; }

        public int TotalLessons { get; set; }
        public int CompletedLessons { get; set; }
    }
}
