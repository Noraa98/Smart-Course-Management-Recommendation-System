using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SmartCourses.BLL.Models.DTOs.Response_ResultDTOs;
using SmartCourses.BLL.Models.DTOs.User_AuthenticationDTOs;
using SmartCourses.BLL.Services.Interfaces.Auth;
using SmartCourses.DAL.Entities.Identity;

namespace SmartCourses.BLL.Services.Implementations.AuthImplmentation
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper _mapper;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        public async Task<ServiceResult<UserDto>> RegisterAsync(RegisterDto registerDto, string role = "Student")
        {
            try
            {
                // Check if email already exists
                var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
                if (existingUser != null)
                {
                    return ServiceResult<UserDto>.Failure("Email is already registered");
                }

                // Map DTO to entity
                var user = _mapper.Map<ApplicationUser>(registerDto);
                user.CreatedAt = DateTime.UtcNow;

                // Create user
                var result = await _userManager.CreateAsync(user, registerDto.Password);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ServiceResult<UserDto>.Failure(errors);
                }

                // Assign role
                var roleResult = await _userManager.AddToRoleAsync(user, role);
                if (!roleResult.Succeeded)
                {
                    // Rollback user creation
                    await _userManager.DeleteAsync(user);
                    return ServiceResult<UserDto>.Failure("Failed to assign role");
                }

                // Map to DTO
                var userDto = _mapper.Map<UserDto>(user);
                userDto.Roles = new List<string> { role };

                return ServiceResult<UserDto>.Success(userDto, "Registration successful");
            }
            catch (Exception ex)
            {
                return ServiceResult<UserDto>.Failure($"An error occurred during registration: {ex.Message}");
            }
        }

        public async Task<ServiceResult<UserDto>> LoginAsync(LoginDto loginDto)
        {
            try
            {
                // Find user by email
                var user = await _userManager.FindByEmailAsync(loginDto.Email);
                if (user == null)
                {
                    return ServiceResult<UserDto>.Failure("Invalid email or password");
                }

                // Attempt sign in
                var result = await _signInManager.PasswordSignInAsync(
                    user.UserName!,
                    loginDto.Password,
                    loginDto.RememberMe,
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    // Update last login
                    user.LastLoginAt = DateTime.UtcNow;
                    await _userManager.UpdateAsync(user);

                    // Get user roles
                    var roles = await _userManager.GetRolesAsync(user);

                    // Map to DTO
                    var userDto = _mapper.Map<UserDto>(user);
                    userDto.Roles = roles.ToList();

                    return ServiceResult<UserDto>.Success(userDto, "Login successful");
                }

                if (result.IsLockedOut)
                {
                    return ServiceResult<UserDto>.Failure("Account is locked. Please try again later.");
                }

                if (result.IsNotAllowed)
                {
                    return ServiceResult<UserDto>.Failure("Account is not allowed to sign in. Please confirm your email.");
                }

                return ServiceResult<UserDto>.Failure("Invalid email or password");
            }
            catch (Exception ex)
            {
                return ServiceResult<UserDto>.Failure($"An error occurred during login: {ex.Message}");
            }
        }

        public async Task<ServiceResult> LogoutAsync()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return ServiceResult.Success("Logout successful");
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"An error occurred during logout: {ex.Message}");
            }
        }

        public async Task<ServiceResult<UserDto>> GetCurrentUserAsync()
        {
            try
            {
                var user = await _userManager.GetUserAsync(_signInManager.Context.User);

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

        public async Task<ServiceResult> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return ServiceResult.Failure("User not found");
                }

                var result = await _userManager.ChangePasswordAsync(
                    user,
                    changePasswordDto.CurrentPassword,
                    changePasswordDto.NewPassword);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ServiceResult.Failure(errors);
                }

                return ServiceResult.Success("Password changed successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> IsEmailAvailableAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                var isAvailable = user == null;

                return ServiceResult<bool>.Success(isAvailable);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}