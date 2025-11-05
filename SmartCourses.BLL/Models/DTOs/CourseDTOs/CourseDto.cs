using SmartCourses.BLL.Models.DTOs.BaseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCourses.BLL.Models.DTOs.CourseDTOs
{
    public class CourseDto : BaseAuditableDto<int>
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public string? ThumbnailPath { get; set; }
        public string Level { get; set; } = string.Empty; // Beginner, Intermediate, Advanced
        public bool IsPublished { get; set; }
        public decimal? Price { get; set; }
        public int DurationInHours { get; set; }

        // Category Info
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;

        // Instructor Info
        public string InstructorId { get; set; } = string.Empty;
        public string InstructorName { get; set; } = string.Empty;

        // Statistics
        public int EnrollmentCount { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public int TotalLessons { get; set; }

        // Related Data
        public List<SkillDto> Skills { get; set; } = new();
        public List<SectionDto> Sections { get; set; } = new();
    }
}
