using System.ComponentModel.DataAnnotations;

namespace SmartCourses.BLL.Models.DTOs.Enrollment_ReviewDTOs
{
    public class LessonProgressUpdateDto
    {
        [Required]
        public int EnrollmentId { get; set; }



        [Required]
        public int LessonId { get; set; }

        public bool IsCompleted { get; set; }



        [Range(0, int.MaxValue)]
        public int WatchedSeconds { get; set; }
    }
}
