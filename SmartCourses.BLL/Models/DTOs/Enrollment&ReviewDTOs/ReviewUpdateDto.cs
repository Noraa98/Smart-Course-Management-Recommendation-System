using System.ComponentModel.DataAnnotations;

namespace SmartCourses.BLL.Models.DTOs.Enrollment_ReviewDTOs
{
    public class ReviewUpdateDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }



        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
        public string? Comment { get; set; }
    }
}
