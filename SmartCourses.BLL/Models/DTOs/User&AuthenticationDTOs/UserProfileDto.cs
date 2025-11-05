using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCourses.BLL.Models.DTOs.User_AuthenticationDTOs
{
    public class UserProfileDto
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;



        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;



        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;



        [StringLength(500)]
        public string? Bio { get; set; }

        public string? ProfilePicturePath { get; set; }
        public List<int> SkillIds { get; set; } = new();
    }
}
