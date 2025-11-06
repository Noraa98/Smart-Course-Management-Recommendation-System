using SmartCourses.BLL.Models.DTOs.Response_ResultDTOs;
using SmartCourses.BLL.Models.DTOs;
using SmartCourses.DAL.Entities;

namespace SmartCourses.BLL.Services.Contracts
{
    public interface ICategoryService 
    {
        Task<ServiceResult<CategoryDto>> GetByIdAsync(int id);
        Task<ServiceResult<List<CategoryDto>>> GetAllAsync();
        Task<ServiceResult<PaginatedResultDto<CategoryDto>>> GetPagedAsync(int pageNumber, int pageSize);
        Task<ServiceResult<CategoryDto>> CreateAsync(CategoryCreateDto createDto, string currentUserId);
        Task<ServiceResult<CategoryDto>> UpdateAsync(CategoryUpdateDto updateDto, string currentUserId);
        Task<ServiceResult> DeleteAsync(int id);
        Task<ServiceResult<CategoryDto>> GetCategoryWithCoursesAsync(int id);

    }
}
