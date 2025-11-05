using SmartCourses.BLL.Models.DTOs.BaseDTOs;
using System.ComponentModel.DataAnnotations;

namespace SmartCourses.BLL.Models.DTOs.CourseDTOs
{
    public class LessonDto : BaseAuditableDto<int>
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public string ContentType { get; set; } = string.Empty; // Video, Article, PDF, Quiz
        public string? ContentPath { get; set; }
        public string? ExternalUrl { get; set; }
        public int DurationInMinutes { get; set; }
        public int Order { get; set; }
        public bool IsFree { get; set; }
        public int SectionId { get; set; }
    }
}