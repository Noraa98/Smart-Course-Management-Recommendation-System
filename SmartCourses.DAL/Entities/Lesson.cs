using SmartCourses.DAL.Common.Entities;
using SmartCourses.DAL.Common.Enums;


namespace SmartCourses.DAL.Entities
{
    public class Lesson : BaseAuditableEntity<int>
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        public ContentType ContentType { get; set; }
        public string? ContentPath { get; set; }
        public string? ExternalUrl { get; set; }
        public int DurationInMinutes { get; set; }
        public int Order { get; set; }
        public bool IsFree { get; set; } = false;

        // Foreign Keys
        public int SectionId { get; set; }

        // Navigation Properties
        public virtual Section Section { get; set; } = null!;
        public virtual ICollection<LessonProgress> LessonProgresses { get; set; } = new HashSet<LessonProgress>();

    }
}
