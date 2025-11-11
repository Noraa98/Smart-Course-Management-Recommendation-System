using SmartCourses.BLL.Models.DTOs.CourseDTOs;

namespace SmartCourses.BLL.Models.DTOs.Enrollment_ReviewDTOs
{
    public class EnrollmentDetailsDto : EnrollmentDto
    {
        public CourseDto Course { get; set; } = null!;
        public List<LessonProgressDto> LessonProgresses { get; set; } = new();
    }
}