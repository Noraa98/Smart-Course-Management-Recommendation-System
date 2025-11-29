using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCourses.BLL.Services.Contracts;
using SmartCourses.BLL.Services.Interfaces;
using System.Security.Claims;

namespace SmartCourses.PL.Areas.Instructor.InstructorArea
{
    [Area("Instructor")]
    [Authorize(Roles = "Instructor")]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly ICourseService _courseService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(
            IDashboardService dashboardService,
            ICourseService courseService,
            ILogger<DashboardController> logger)
        {
            _dashboardService = dashboardService;
            _courseService = courseService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _dashboardService.GetInstructorDashboardAsync(userId!);

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Errors.FirstOrDefault();
                return View();
            }

            return View(result.Data);
        }
    }
}
