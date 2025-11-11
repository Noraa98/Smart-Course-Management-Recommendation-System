using System.ComponentModel.DataAnnotations;

namespace SmartCourses.BLL.Models.DTOs.CourseDTOs
{
    public class LessonCreateDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        


        [StringLength(1000)]
        public string? Description { get; set; }



        [Required]
        public int ContentType { get; set; } // 1=Video, 2=Article, 3=PDF, 4=Quiz



        public string? ContentPath { get; set; }
        public string? ExternalUrl { get; set; }

        [Required]
        [Range(1, 1440)]
        public int DurationInMinutes { get; set; }

        public int Order { get; set; }
        public bool IsFree { get; set; }

        [Required]
        public int SectionId { get; set; }
    }
}
