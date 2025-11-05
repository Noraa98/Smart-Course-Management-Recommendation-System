using SmartCourses.DAL.Common.Entities;

namespace SmartCourses.DAL.Entities
{
    public class LessonProgress : BaseAuditableEntity<int>
    {
        public bool IsCompleted { get; set; } = false;
        public DateTime? CompletedAt { get; set; }
        public int WatchedSeconds { get; set; } = 0;

        // Foreign Keys
        public int EnrollmentId { get; set; }
        public int LessonId { get; set; }

        // Navigation Properties
        public virtual Enrollment Enrollment { get; set; } = null!;
        public virtual Lesson Lesson { get; set; } = null!;

    }
}
