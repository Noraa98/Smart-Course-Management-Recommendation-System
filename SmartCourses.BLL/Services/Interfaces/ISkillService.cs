using SmartCourses.BLL.Models.DTOs.Response_ResultDTOs;
using SmartCourses.BLL.Models.DTOs;
using SmartCourses.DAL.Entities;

namespace SmartCourses.BLL.Services.Contracts
{
    public interface ISkillService 
    {
        Task<ServiceResult<SkillDto>> GetByIdAsync(int id);
        Task<ServiceResult<List<SkillDto>>> GetAllAsync();
        Task<ServiceResult<PaginatedResultDto<SkillDto>>> GetPagedAsync(int pageNumber, int pageSize);
        Task<ServiceResult<SkillDto>> CreateAsync(SkillCreateDto createDto, string currentUserId);
        Task<ServiceResult<SkillDto>> UpdateAsync(int id, SkillCreateDto updateDto, string currentUserId);
        Task<ServiceResult> DeleteAsync(int id);
        Task<ServiceResult<List<SkillDto>>> GetPopularSkillsAsync(int count = 10);
        Task<ServiceResult<List<SkillDto>>> GetCourseSkillsAsync(int courseId);

    }
}