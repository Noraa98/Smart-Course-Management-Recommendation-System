using SmartCourses.BLL.Models.DTOs.CourseDTOs;

namespace SmartCourses.PL.ViewModels
{
    public class EnrollmentDetailsViewModel
    {
        public int EnrollmentId { get; set; }
        public CourseDto Course { get; set; }
        public LessonDto CurrentLesson { get; set; }
        public int? PreviousLessonId { get; set; }
        public int? NextLessonId { get; set; }
        public int TotalLessons { get; set; }
        public int CompletedLessons { get; set; }
        public decimal ProgressPercent { get; set; }
        public List<int> CompletedLessonIds { get; set; } = new();

    }
}
