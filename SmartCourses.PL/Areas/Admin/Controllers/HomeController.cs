using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmartCourses.PL.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class HomeController : Controller
	{
		[HttpGet]
		public IActionResult Index()
		{
			return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
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


