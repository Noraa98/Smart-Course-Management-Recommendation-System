using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmartCourses.PL.Controllers.AdminArea
{
	[Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class CourseController : Controller
	{
		// Handle /Admin/Course by redirecting to CoursesController (plural)
		[HttpGet]
		public IActionResult Index()
		{
			return RedirectToAction("Index", "Courses", new { area = "Admin" });
		}
	}
}


