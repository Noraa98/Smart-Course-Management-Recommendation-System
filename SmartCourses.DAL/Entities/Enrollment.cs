using SmartCourses.DAL.Common.Entities;
using SmartCourses.DAL.Entities.Identity;

namespace SmartCourses.DAL.Entities
{
    public class Enrollment : BaseAuditableEntity<int>
    {
        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
        public decimal ProgressPercent { get; set; } = 0;
        public DateTime? CompletedAt { get; set; }
        public bool IsCompleted { get; set; } = false;

        // Foreign Keys
        public string UserId { get; set; } = string.Empty;
        public int CourseId { get; set; }

        // Navigation Properties
        public virtual ApplicationUser User { get; set; } = null!;
        public virtual Course Course { get; set; } = null!;
        public virtual ICollection<LessonProgress> LessonProgresses { get; set; } = new HashSet<LessonProgress>();

    }
}
