using System.ComponentModel.DataAnnotations;

namespace SmartCourses.BLL.Models.DTOs.CourseDTOs
{
    public class SectionCreateDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public int Order { get; set; }
        public int CourseId { get; set; }
    }
}
