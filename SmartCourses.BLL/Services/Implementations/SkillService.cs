using AutoMapper;
using SmartCourses.BLL.Models.DTOs;
using SmartCourses.BLL.Models.DTOs.Response_ResultDTOs;
using SmartCourses.BLL.Services.Contracts;
using SmartCourses.DAL.Contracts;
using SmartCourses.DAL.Entities;

namespace SmartCourses.BLL.Services.Implementations
{
    public class SkillService : ISkillService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SkillService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResult<SkillDto>> GetByIdAsync(int id)
        {
            try
            {
                var skill = await _unitOfWork.Skills.GetByIdAsync(id);
                if (skill == null)
                {
                    return ServiceResult<SkillDto>.Failure("Skill not found");
                }

                var skillDto = _mapper.Map<SkillDto>(skill);
                return ServiceResult<SkillDto>.Success(skillDto);
            }
            catch (Exception ex)
            {
                return ServiceResult<SkillDto>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<SkillDto>>> GetAllAsync()
        {
            try
            {
                var skills = await _unitOfWork.Skills.GetSkillsWithCoursesAsync();
                var skillDtos = _mapper.Map<List<SkillDto>>(skills);

                return ServiceResult<List<SkillDto>>.Success(skillDtos);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<SkillDto>>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<PaginatedResultDto<SkillDto>>> GetPagedAsync(int pageNumber, int pageSize)
        {
            try
            {
                var (skills, totalCount) = await _unitOfWork.Skills.GetPagedAsync(
                    pageNumber,
                    pageSize,
                    orderBy: q => q.OrderBy(s => s.Name));

                var skillDtos = _mapper.Map<List<SkillDto>>(skills);
                var paginatedResult = new PaginatedResultDto<SkillDto>(
                    skillDtos,
                    totalCount,
                    pageNumber,
                    pageSize);

                return ServiceResult<PaginatedResultDto<SkillDto>>.Success(paginatedResult);
            }
            catch (Exception ex)
            {
                return ServiceResult<PaginatedResultDto<SkillDto>>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<SkillDto>> CreateAsync(SkillCreateDto createDto, string currentUserId)
        {
            try
            {
                var existing = await _unitOfWork.Skills.FirstOrDefaultAsync(s => s.Name == createDto.Name);
                if (existing != null)
                {
                    return ServiceResult<SkillDto>.Failure("Skill name already exists");
                }

                var skill = _mapper.Map<Skill>(createDto);
                skill.CreatedBy = currentUserId;
                skill.LastModifiedBy = currentUserId;
                skill.CreatedOn = DateTime.UtcNow;
                skill.LastModifiedOn = DateTime.UtcNow;

                await _unitOfWork.Skills.AddAsync(skill);
                await _unitOfWork.SaveChangesAsync();

                var skillDto = _mapper.Map<SkillDto>(skill);
                return ServiceResult<SkillDto>.Success(skillDto, "Skill created successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<SkillDto>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<SkillDto>> UpdateAsync(int id, SkillCreateDto updateDto, string currentUserId)
        {
            try
            {
                var skill = await _unitOfWork.Skills.GetByIdAsync(id);
                if (skill == null)
                {
                    return ServiceResult<SkillDto>.Failure("Skill not found");
                }

                var existing = await _unitOfWork.Skills.FirstOrDefaultAsync(
                    s => s.Name == updateDto.Name && s.Id != id);

                if (existing != null)
                {
                    return ServiceResult<SkillDto>.Failure("Skill name already exists");
                }

                skill.Name = updateDto.Name;
                skill.Description = updateDto.Description;
                skill.LastModifiedBy = currentUserId;
                skill.LastModifiedOn = DateTime.UtcNow;

                _unitOfWork.Skills.Update(skill);
                await _unitOfWork.SaveChangesAsync();

                var skillDto = _mapper.Map<SkillDto>(skill);
                return ServiceResult<SkillDto>.Success(skillDto, "Skill updated successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<SkillDto>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            try
            {
                var skill = await _unitOfWork.Skills.GetByIdAsync(id);
                if (skill == null)
                {
                    return ServiceResult.Failure("Skill not found");
                }

                _unitOfWork.Skills.SoftDelete(skill);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult.Success("Skill deleted successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<SkillDto>>> GetPopularSkillsAsync(int count = 10)
        {
            try
            {
                var skills = await _unitOfWork.Skills.GetPopularSkillsAsync(count);
                var skillDtos = _mapper.Map<List<SkillDto>>(skills);

                return ServiceResult<List<SkillDto>>.Success(skillDtos);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<SkillDto>>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<SkillDto>>> GetCourseSkillsAsync(int courseId)
        {
            try
            {
                var skills = await _unitOfWork.Skills.GetCourseSkillsAsync(courseId);
                var skillDtos = _mapper.Map<List<SkillDto>>(skills);

                return ServiceResult<List<SkillDto>>.Success(skillDtos);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<SkillDto>>.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}