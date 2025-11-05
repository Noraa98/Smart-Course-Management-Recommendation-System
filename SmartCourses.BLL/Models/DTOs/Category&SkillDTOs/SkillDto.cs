using SmartCourses.BLL.Models.DTOs.BaseDTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCourses.BLL.Models.DTOs
{
    public class SkillDto : BaseAuditableDto<int>
    {
        [Required(ErrorMessage = "Skill name is required")]
        [StringLength(100, ErrorMessage = "Skill name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;



        [StringLength(300, ErrorMessage = "Description cannot exceed 300 characters")]
        public string? Description { get; set; }

        public int CourseCount { get; set; }
        public int UserCount { get; set; }
    }
}