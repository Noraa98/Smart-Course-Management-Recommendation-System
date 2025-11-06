using AutoMapper;
using SmartCourses.BLL.Models.DTOs;
using SmartCourses.BLL.Models.DTOs.Response_ResultDTOs;
using SmartCourses.BLL.Services.Contracts;
using SmartCourses.DAL.Contracts;
using SmartCourses.DAL.Entities;

namespace SmartCourses.BLL.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResult<CategoryDto>> GetByIdAsync(int id)
        {
            try
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(id);

                if (category == null)
                {
                    return ServiceResult<CategoryDto>.Failure("Category not found");
                }

                var categoryDto = _mapper.Map<CategoryDto>(category);
                return ServiceResult<CategoryDto>.Success(categoryDto);
            }
            catch (Exception ex)
            {
                return ServiceResult<CategoryDto>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<CategoryDto>>> GetAllAsync()
        {
            try
            {
                var categories = await _unitOfWork.Categories.GetCategoriesWithCourseCountAsync();
                var categoryDtos = _mapper.Map<List<CategoryDto>>(categories);

                return ServiceResult<List<CategoryDto>>.Success(categoryDtos);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<CategoryDto>>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<PaginatedResultDto<CategoryDto>>> GetPagedAsync(int pageNumber, int pageSize)
        {
            try
            {
                var (categories, totalCount) = await _unitOfWork.Categories.GetPagedAsync(
                    pageNumber,
                    pageSize,
                    orderBy: q => q.OrderBy(c => c.Name));

                var categoryDtos = _mapper.Map<List<CategoryDto>>(categories);
                var paginatedResult = new PaginatedResultDto<CategoryDto>(
                    categoryDtos,
                    totalCount,
                    pageNumber,
                    pageSize);

                return ServiceResult<PaginatedResultDto<CategoryDto>>.Success(paginatedResult);
            }
            catch (Exception ex)
            {
                return ServiceResult<PaginatedResultDto<CategoryDto>>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<CategoryDto>> CreateAsync(CategoryCreateDto createDto, string currentUserId)
        {
            try
            {
                // Check if category name already exists
                var existing = await _unitOfWork.Categories.FirstOrDefaultAsync(c => c.Name == createDto.Name);
                if (existing != null)
                {
                    return ServiceResult<CategoryDto>.Failure("Category name already exists");
                }

                var category = _mapper.Map<Category>(createDto);
                category.CreatedBy = currentUserId;
                category.LastModifiedBy = currentUserId;
                category.CreatedOn = DateTime.UtcNow;
                category.LastModifiedOn = DateTime.UtcNow;

                await _unitOfWork.Categories.AddAsync(category);
                await _unitOfWork.SaveChangesAsync();

                var categoryDto = _mapper.Map<CategoryDto>(category);
                return ServiceResult<CategoryDto>.Success(categoryDto, "Category created successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<CategoryDto>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<CategoryDto>> UpdateAsync(CategoryUpdateDto updateDto, string currentUserId)
        {
            try
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(updateDto.Id);
                if (category == null)
                {
                    return ServiceResult<CategoryDto>.Failure("Category not found");
                }

                // Check if new name conflicts with existing category
                var existing = await _unitOfWork.Categories.FirstOrDefaultAsync(
                    c => c.Name == updateDto.Name && c.Id != updateDto.Id);

                if (existing != null)
                {
                    return ServiceResult<CategoryDto>.Failure("Category name already exists");
                }

                _mapper.Map(updateDto, category);
                category.LastModifiedBy = currentUserId;
                category.LastModifiedOn = DateTime.UtcNow;

                _unitOfWork.Categories.Update(category);
                await _unitOfWork.SaveChangesAsync();

                var categoryDto = _mapper.Map<CategoryDto>(category);
                return ServiceResult<CategoryDto>.Success(categoryDto, "Category updated successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<CategoryDto>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            try
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(id);
                if (category == null)
                {
                    return ServiceResult.Failure("Category not found");
                }

                // Check if category has courses
                var hasCourses = await _unitOfWork.Categories.CategoryHasCoursesAsync(id);
                if (hasCourses)
                {
                    return ServiceResult.Failure("Cannot delete category with existing courses");
                }

                _unitOfWork.Categories.SoftDelete(category);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult.Success("Category deleted successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<CategoryDto>> GetCategoryWithCoursesAsync(int id)
        {
            try
            {
                var category = await _unitOfWork.Categories.GetCategoryWithCoursesAsync(id);
                if (category == null)
                {
                    return ServiceResult<CategoryDto>.Failure("Category not found");
                }

                var categoryDto = _mapper.Map<CategoryDto>(category);
                return ServiceResult<CategoryDto>.Success(categoryDto);
            }
            catch (Exception ex)
            {
                return ServiceResult<CategoryDto>.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}
