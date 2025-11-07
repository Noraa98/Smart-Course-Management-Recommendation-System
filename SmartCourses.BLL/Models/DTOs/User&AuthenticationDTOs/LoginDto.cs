using System.ComponentModel.DataAnnotations;

namespace SmartCourses.BLL.Models.DTOs.User_AuthenticationDTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;


        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;


        public bool RememberMe { get; set; }
    }
}
