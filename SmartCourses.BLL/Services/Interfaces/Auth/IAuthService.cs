using SmartCourses.BLL.Models.DTOs.Response_ResultDTOs;
using SmartCourses.BLL.Models.DTOs.User_AuthenticationDTOs;

namespace SmartCourses.BLL.Services.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<ServiceResult<UserDto>> RegisterAsync(RegisterDto registerDto, string role = "Student");
        Task<ServiceResult<UserDto>> LoginAsync(LoginDto loginDto);
        Task<ServiceResult> LogoutAsync();
        Task<ServiceResult<UserDto>> GetCurrentUserAsync();
        Task<ServiceResult> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto);
        Task<ServiceResult<bool>> IsEmailAvailableAsync(string email);

    }
}
