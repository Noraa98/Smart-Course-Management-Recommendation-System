using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCourses.BLL.Services.Interfaces.Auth;

namespace SmartCourses.PL.Controllers.AdminArea
{
	[Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class AccountController : Controller
	{
		private readonly IAuthService _authService;
		private readonly ILogger<AccountController> _logger;

		public AccountController(IAuthService authService, ILogger<AccountController> logger)
		{
			_authService = authService;
			_logger = logger;
		}

		// Allow GET logout for area route: /Admin/Account/Logout
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


