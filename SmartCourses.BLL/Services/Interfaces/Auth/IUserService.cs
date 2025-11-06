using SmartCourses.BLL.Models.DTOs;
using SmartCourses.BLL.Models.DTOs.Response_ResultDTOs;
using SmartCourses.BLL.Models.DTOs.User_AuthenticationDTOs;

namespace SmartCourses.BLL.Services.Interfaces.Auth
{
    public interface IUserService
    {
        Task<ServiceResult<UserDto>> GetUserByIdAsync(string userId);
        Task<ServiceResult<UserDto>> GetUserByEmailAsync(string email);
        Task<ServiceResult<PaginatedResultDto<UserDto>>> GetAllUsersAsync(int pageNumber, int pageSize, string? role = null);
        Task<ServiceResult<UserDto>> UpdateProfileAsync(UserProfileDto profileDto);
        Task<ServiceResult> AddUserSkillsAsync(string userId, List<int> skillIds);
        Task<ServiceResult> RemoveUserSkillAsync(string userId, int skillId);
        Task<ServiceResult<List<UserSkillDto>>> GetUserSkillsAsync(string userId);

    }
}
