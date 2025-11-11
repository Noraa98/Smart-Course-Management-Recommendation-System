using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmartCourses.PL.Controllers.AdminArea
{
	[Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class CourseController : Controller
	{
		// Handle /Admin/Course by redirecting to Admin Dashboard (no Admin Course controller exists)
		[HttpGet]
		public IActionResult Index()
		{
			return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
		}
	}
}


