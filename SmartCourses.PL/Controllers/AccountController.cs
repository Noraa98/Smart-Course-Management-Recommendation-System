using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCourses.BLL.Models.DTOs.User_AuthenticationDTOs;
using SmartCourses.BLL.Services.Interfaces.Auth;
using SmartCourses.BLL.Services.Interfaces;
using System.Security.Claims;

namespace SmartCourses.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly IFileService _fileService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            IAuthService authService,
            IUserService userService,
            IFileService fileService,
            ILogger<AccountController> logger)
        {
            _authService = authService;
            _userService = userService;
            _fileService = fileService;
            _logger = logger;
        }

        
        // Register
        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto model, string role = "Student")
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _authService.RegisterAsync(model, role);

            if (result.IsSuccess)
            {
                TempData["Success"] = "Registration successful! Please login.";
                return RedirectToAction(nameof(Login));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return View(model);
        }

        
        // Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _authService.LoginAsync(model);

            if (result.IsSuccess)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                // Redirect based on role
                var roles = result.Data?.Roles ?? new List<string>();
                if (roles.Contains("Admin"))
                {
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
                else if (roles.Contains("Instructor"))
                {
                    return RedirectToAction("Index", "Dashboard", new { area = "Instructor" });
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return View(model);
        }

       
        // Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            TempData["Success"] = "You have been logged out successfully.";
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> LogoutPost()
        {
            await _authService.LogoutAsync();
            TempData["Success"] = "You have been logged out successfully.";
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        // Profile
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction(nameof(Login));
            }

            var result = await _userService.GetUserByIdAsync(userId);

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Errors.FirstOrDefault() ?? "Error loading profile";
                return RedirectToAction("Index", "Home");
            }

            var profileDto = new UserProfileDto
            {
                Id = result.Data!.Id,
                FirstName = result.Data.FirstName,
                LastName = result.Data.LastName,
                Email = result.Data.Email,
                Bio = result.Data.Bio,
                ProfilePicturePath = result.Data.ProfilePicturePath,
                SkillIds = result.Data.Skills.Select(s => s.Id).ToList()
            };

            return View(profileDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Profile(UserProfileDto model, IFormFile? profilePicture)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || userId != model.Id)
            {
                return Forbid();
            }

            // Handle profile picture upload
            if (profilePicture != null && profilePicture.Length > 0)
            {
                var validationResult = await _fileService.ValidateFileAsync(
                    profilePicture,
                    new[] { ".jpg", ".jpeg", ".png", ".gif" },
                    5 * 1024 * 1024); // 5MB

                if (validationResult.IsSuccess)
                {
                    // Delete old picture if exists
                    if (!string.IsNullOrEmpty(model.ProfilePicturePath))
                    {
                        await _fileService.DeleteFileAsync(model.ProfilePicturePath);
                    }

                    // Upload new picture
                    var uploadResult = await _fileService.UploadFileAsync(profilePicture, "profiles");
                    if (uploadResult.IsSuccess)
                    {
                        model.ProfilePicturePath = uploadResult.Data;
                    }
                }
                else
                {
                    ModelState.AddModelError(nameof(profilePicture), validationResult.Errors.FirstOrDefault() ?? "Invalid file");
                    return View(model);
                }
            }

            var result = await _userService.UpdateProfileAsync(model);

            if (result.IsSuccess)
            {
                // Update skills
                if (model.SkillIds != null && model.SkillIds.Any())
                {
                    await _userService.AddUserSkillsAsync(userId, model.SkillIds);
                }

                TempData["Success"] = "Profile updated successfully";
                return RedirectToAction(nameof(Profile));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return View(model);
        }

        
        // Change Password
        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction(nameof(Login));
            }

            var result = await _authService.ChangePasswordAsync(userId, model);

            if (result.IsSuccess)
            {
                TempData["Success"] = "Password changed successfully";
                return RedirectToAction(nameof(Profile));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return View(model);
        }

        
        // Access Denied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}