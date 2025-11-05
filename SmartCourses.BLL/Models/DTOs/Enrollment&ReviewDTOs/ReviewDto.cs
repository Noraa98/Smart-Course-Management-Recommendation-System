using SmartCourses.BLL.Models.DTOs.BaseDTOs;

namespace SmartCourses.BLL.Models.DTOs.Enrollment_ReviewDTOs
{
    public class ReviewDto : BaseAuditableDto<int>
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? UserProfilePicture { get; set; }

        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;

        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}