using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartCourses.BLL.Services.Interfaces.Auth;
using SmartCourses.DAL.Entities.Identity;

namespace SmartCourses.PL.Areas.Admin.Controllers
{

        [Area("Admin")]
        [Authorize(Roles = "Admin")]
        public class UsersController : Controller
        {
            private readonly IUserService _userService;
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly ILogger<UsersController> _logger;

            public UsersController(
                IUserService userService,
                UserManager<ApplicationUser> userManager,
                ILogger<UsersController> logger)
            {
                _userService = userService;
                _userManager = userManager;
                _logger = logger;
            }

            // List Users
            [HttpGet]
            public async Task<IActionResult> Index(string? role, int pageNumber = 1, int pageSize = 20)
            {
                var result = await _userService.GetAllUsersAsync(pageNumber, pageSize, role);

                ViewBag.CurrentRole = role;

                if (!result.IsSuccess)
                {
                    TempData["Error"] = result.Errors.FirstOrDefault();
                    return View();
                }

                return View(result.Data);
            }

            
            // User Details
            [HttpGet]
            public async Task<IActionResult> Details(string id)
            {
                var result = await _userService.GetUserByIdAsync(id);

                if (!result.IsSuccess)
                {
                    TempData["Error"] = "User not found";
                    return RedirectToAction(nameof(Index));
                }

                return View(result.Data);
            }

          
            // Manage Roles
            [HttpGet]
            public async Task<IActionResult> ManageRoles(string id)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    TempData["Error"] = "User not found";
                    return RedirectToAction(nameof(Index));
                }

                var userRoles = await _userManager.GetRolesAsync(user);
                var allRoles = new List<string> { "Admin", "Instructor", "Student" };

                ViewBag.User = user;
                ViewBag.UserRoles = userRoles;
                ViewBag.AllRoles = allRoles;

                return View();
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> ManageRoles(string userId, List<string> roles)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    TempData["Error"] = "User not found";
                    return RedirectToAction(nameof(Index));
                }

                // Get current roles
                var userRoles = await _userManager.GetRolesAsync(user);

                // Remove all current roles
                var removeResult = await _userManager.RemoveFromRolesAsync(user, userRoles);
                if (!removeResult.Succeeded)
                {
                    TempData["Error"] = "Failed to update roles";
                    return RedirectToAction(nameof(ManageRoles), new { id = userId });
                }

                // Add new roles
                if (roles != null && roles.Any())
                {
                    var addResult = await _userManager.AddToRolesAsync(user, roles);
                    if (!addResult.Succeeded)
                    {
                        TempData["Error"] = "Failed to add roles";
                        return RedirectToAction(nameof(ManageRoles), new { id = userId });
                    }
                }

                TempData["Success"] = "Roles updated successfully";
                return RedirectToAction(nameof(Details), new { id = userId });
            }

            // Lock/Unlock User
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> LockUser(string id)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    TempData["Error"] = "User not found";
                    return RedirectToAction(nameof(Index));
                }

                var result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));

                if (result.Succeeded)
                {
                    TempData["Success"] = "User locked successfully";
                }
                else
                {
                    TempData["Error"] = "Failed to lock user";
                }

                return RedirectToAction(nameof(Details), new { id });
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> UnlockUser(string id)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    TempData["Error"] = "User not found";
                    return RedirectToAction(nameof(Index));
                }

                var result = await _userManager.SetLockoutEndDateAsync(user, null);

                if (result.Succeeded)
                {
                    TempData["Success"] = "User unlocked successfully";
                }
                else
                {
                    TempData["Error"] = "Failed to unlock user";
                }

                return RedirectToAction(nameof(Details), new { id });
            }

            // Delete User
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Delete(string id)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    TempData["Error"] = "User not found";
                    return RedirectToAction(nameof(Index));
                }

                // Prevent deleting yourself
                if (user.Id == User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value)
                {
                    TempData["Error"] = "You cannot delete your own account";
                    return RedirectToAction(nameof(Index));
                }

                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    TempData["Success"] = "User deleted successfully";
                }
                else
                {
                    TempData["Error"] = "Failed to delete user";
                }

                return RedirectToAction(nameof(Index));
            }
        }
    }
