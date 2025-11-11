using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartCourses.BLL.Models.DTOs;
using SmartCourses.BLL.Models.DTOs.Response_ResultDTOs;
using SmartCourses.BLL.Models.DTOs.User_AuthenticationDTOs;
using SmartCourses.BLL.Services.Interfaces.Auth;
using SmartCourses.DAL.Contracts;
using SmartCourses.DAL.Entities.Identity;
using SmartCourses.DAL.Entities.RelationshipsTables;

namespace SmartCourses.BLL.Services.Implementations.AuthImplmentation
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(
            UserManager<ApplicationUser> userManager,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResult<UserDto>> GetUserByIdAsync(string userId)
        {
            try
            {
                var user = await _userManager.Users
                    .Include(u => u.UserSkills)
                        .ThenInclude(us => us.Skill)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    return ServiceResult<UserDto>.Failure("User not found");
                }

                var roles = await _userManager.GetRolesAsync(user);
                var userDto = _mapper.Map<UserDto>(user);
                userDto.Roles = roles.ToList();

                return ServiceResult<UserDto>.Success(userDto);
            }
            catch (Exception ex)
            {
                return ServiceResult<UserDto>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<UserDto>> GetUserByEmailAsync(string email)
        {
            try
            {
                var user = await _userManager.Users
                    .Include(u => u.UserSkills)
                        .ThenInclude(us => us.Skill)
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    return ServiceResult<UserDto>.Failure("User not found");
                }

                var roles = await _userManager.GetRolesAsync(user);
                var userDto = _mapper.Map<UserDto>(user);
                userDto.Roles = roles.ToList();

                return ServiceResult<UserDto>.Success(userDto);
            }
            catch (Exception ex)
            {
                return ServiceResult<UserDto>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<PaginatedResultDto<UserDto>>> GetAllUsersAsync(
            int pageNumber,
            int pageSize,
            string? role = null)
        {
            try
            {
                IQueryable<ApplicationUser> query = _userManager.Users
                    .Include(u => u.UserSkills)
                        .ThenInclude(us => us.Skill);

                // Filter by role if specified
                if (!string.IsNullOrEmpty(role))
                {
                    var usersInRole = await _userManager.GetUsersInRoleAsync(role);
                    var userIds = usersInRole.Select(u => u.Id).ToList();
                    query = query.Where(u => userIds.Contains(u.Id));
                }

                var totalCount = await query.CountAsync();

                var users = await query
                    .OrderBy(u => u.FirstName)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var userDtos = new List<UserDto>();
                foreach (var user in users)
                {
                    var userDto = _mapper.Map<UserDto>(user);
                    var userRoles = await _userManager.GetRolesAsync(user);
                    userDto.Roles = userRoles.ToList();
                    userDtos.Add(userDto);
                }

                var paginatedResult = new PaginatedResultDto<UserDto>(
                    userDtos,
                    totalCount,
                    pageNumber,
                    pageSize);

                return ServiceResult<PaginatedResultDto<UserDto>>.Success(paginatedResult);
            }
            catch (Exception ex)
            {
                return ServiceResult<PaginatedResultDto<UserDto>>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<UserDto>> UpdateProfileAsync(UserProfileDto profileDto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(profileDto.Id);
                if (user == null)
                {
                    return ServiceResult<UserDto>.Failure("User not found");
                }

                // Update basic info
                user.FirstName = profileDto.FirstName;
                user.LastName = profileDto.LastName;
                user.Bio = profileDto.Bio;
                user.ProfilePicturePath = profileDto.ProfilePicturePath;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ServiceResult<UserDto>.Failure(errors);
                }

                // Update email if changed
                if (user.Email != profileDto.Email)
                {
                    var emailResult = await _userManager.SetEmailAsync(user, profileDto.Email);
                    if (!emailResult.Succeeded)
                    {
                        var errors = emailResult.Errors.Select(e => e.Description).ToList();
                        return ServiceResult<UserDto>.Failure(errors);
                    }
                }

                var roles = await _userManager.GetRolesAsync(user);
                var userDto = _mapper.Map<UserDto>(user);
                userDto.Roles = roles.ToList();

                return ServiceResult<UserDto>.Success(userDto, "Profile updated successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<UserDto>.Failure($"An error occurred: {ex.Message}");
            }
        }

        
        public async Task<ServiceResult> AddUserSkillsAsync(string userId, List<int> skillIds)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return ServiceResult.Failure("User not found");
                }

                // Get existing skills using DbContext directly
                var existingSkills = await _unitOfWork.Repository<UserSkill, (string, int)>()
                    .FindAsync(us => us.UserId == userId);

                var existingSkillIds = existingSkills.Select(s => s.SkillId).ToList();
                var newSkillIds = skillIds.Except(existingSkillIds).ToList();

                foreach (var skillId in newSkillIds)
                {
                    var skill = await _unitOfWork.Skills.GetByIdAsync(skillId);
                    if (skill != null)
                    {
                        var userSkill = new UserSkill
                        {
                            UserId = userId,
                            SkillId = skillId,
                            ProficiencyLevel = 1,
                            AddedAt = DateTime.UtcNow
                        };

                     
                        await _unitOfWork.Repository<UserSkill, (string, int)>().AddAsync(userSkill);
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                return ServiceResult.Success("Skills added successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult> RemoveUserSkillAsync(string userId, int skillId)
        {
            try
            {
                var userSkillRepo = _unitOfWork.Repository<UserSkill, (string, int)>();

                var userSkill = await userSkillRepo.FirstOrDefaultAsync(
                    us => us.UserId == userId && us.SkillId == skillId);

                if (userSkill == null)
                {
                    return ServiceResult.Failure("User skill not found");
                }

                userSkillRepo.Delete(userSkill);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult.Success("Skill removed successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<UserSkillDto>>> GetUserSkillsAsync(string userId)
        {
            try
            {
                var userSkills = await _unitOfWork.Repository<UserSkill, (string, int)>()
                    .FindAsync(
                        us => us.UserId == userId,
                        us => us.Skill);

                var userSkillDtos = _mapper.Map<List<UserSkillDto>>(userSkills);

                return ServiceResult<List<UserSkillDto>>.Success(userSkillDtos);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<UserSkillDto>>.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}