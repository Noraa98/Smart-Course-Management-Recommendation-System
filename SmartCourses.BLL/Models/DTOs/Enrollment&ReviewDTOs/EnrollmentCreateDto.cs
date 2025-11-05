using System.ComponentModel.DataAnnotations;

namespace SmartCourses.BLL.Models.DTOs.Enrollment_ReviewDTOs
{
    public class EnrollmentCreateDto
    {
        [Required(ErrorMessage = "Course ID is required")]
        public int CourseId { get; set; }
    }
}
