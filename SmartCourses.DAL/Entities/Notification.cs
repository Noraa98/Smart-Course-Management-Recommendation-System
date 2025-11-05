using SmartCourses.DAL.Common.Entities;
using SmartCourses.DAL.Entities.Identity;

namespace SmartCourses.DAL.Entities
{
    public class Notification : BaseAuditableEntity<int>
    {
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
        public string? Link { get; set; }

        // Foreign Keys
        public string UserId { get; set; } = string.Empty;

        // Navigation Properties
        public virtual ApplicationUser User { get; set; } = null!;

    }
}
