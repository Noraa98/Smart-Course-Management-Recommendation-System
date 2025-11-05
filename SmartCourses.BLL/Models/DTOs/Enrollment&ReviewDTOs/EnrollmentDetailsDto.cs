using SmartCourses.BLL.Models.DTOs.CourseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCourses.BLL.Models.DTOs.Enrollment_ReviewDTOs
{
    public class EnrollmentDetailsDto : EnrollmentDto
    {
        public CourseDto Course { get; set; } = null!;
        public List<LessonProgressDto> LessonProgresses { get; set; } = new();
    }
}