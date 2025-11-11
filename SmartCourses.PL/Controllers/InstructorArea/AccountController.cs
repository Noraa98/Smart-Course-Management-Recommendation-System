using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCourses.BLL.Services.Interfaces.Auth;

namespace SmartCourses.PL.Controllers.InstructorArea
{
	[Area("Instructor")]
	[Authorize(Roles = "Instructor")]
	public class AccountController : Controller
	{
		private readonly IAuthService _authService;
		private readonly ILogger<AccountController> _logger;

		public AccountController(IAuthService authService, ILogger<AccountController> logger)
		{
			_authService = authService;
			_logger = logger;
		}

		// Allow GET logout for area route: /Instructor/Account/Logout
		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> Logout()
		{
			await _authService.LogoutAsync();
			TempData["Success"] = "You have been logged out successfully.";
			return RedirectToAction("Index", "Home", new { area = "" });
		}
	}
}


