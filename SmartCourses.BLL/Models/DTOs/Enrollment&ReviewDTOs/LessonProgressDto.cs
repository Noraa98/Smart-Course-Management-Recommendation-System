using SmartCourses.BLL.Models.DTOs.BaseDTOs;

namespace SmartCourses.BLL.Models.DTOs.Enrollment_ReviewDTOs
{
    public class LessonProgressDto : BaseAuditableDto<int>
    {
        public int EnrollmentId { get; set; }
        public int LessonId { get; set; }
        public string LessonTitle { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int WatchedSeconds { get; set; }
        public int TotalSeconds { get; set; }
    }
}