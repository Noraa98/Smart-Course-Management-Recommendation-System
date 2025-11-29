using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmartCourses.PL.Areas.Instructor.InstructorArea
{
	[Area("Instructor")]
	[Authorize(Roles = "Instructor")]
	public class HomeController : Controller
	{
		[HttpGet]
		public IActionResult Index()
		{
			return RedirectToAction("Index", "Dashboard", new { area = "Instructor" });
		}

		[HttpGet]
		public IActionResult About()
		{
			return View("~/Views/Home/About.cshtml");
		}

		[HttpGet]
		public IActionResult Contact()
		{
			return View("~/Views/Home/Contact.cshtml");
		}
	}
}


