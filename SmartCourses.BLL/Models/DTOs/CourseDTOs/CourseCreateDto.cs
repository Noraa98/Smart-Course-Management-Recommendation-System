using System.ComponentModel.DataAnnotations;

namespace SmartCourses.BLL.Models.DTOs.CourseDTOs
{
    public class CourseCreateDto
    {
        [Required(ErrorMessage = "Course title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;



        [Required(ErrorMessage = "Description is required")]
        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string Description { get; set; } = string.Empty;



        [Required(ErrorMessage = "Short description is required")]
        [StringLength(300, ErrorMessage = "Short description cannot exceed 300 characters")]
        public string ShortDescription { get; set; } = string.Empty;



        public string? ThumbnailPath { get; set; }

        [Required(ErrorMessage = "Level is required")]


        public int Level { get; set; } // 1=Beginner, 2=Intermediate, 3=Advanced

        [Range(0, double.MaxValue, ErrorMessage = "Price must be positive")]
        public decimal? Price { get; set; }



        [Required(ErrorMessage = "Duration is required")]
        [Range(1, 1000, ErrorMessage = "Duration must be between 1 and 1000 hours")]
        public int DurationInHours { get; set; }



        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }

        public List<int> SkillIds { get; set; } = new();
    }
}
