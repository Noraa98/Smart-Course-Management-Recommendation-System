using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCourses.BLL.Services.Contracts;
using SmartCourses.BLL.Services.Interfaces;
using System.Security.Claims;

namespace SmartCourses.PL.Areas.Student.StudentArea
{
    [Area("Student")]
    [Authorize(Roles = "Student")]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly IEnrollmentService _enrollmentService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(
            IDashboardService dashboardService,
            IEnrollmentService enrollmentService,
            ILogger<DashboardController> logger)
        {
            _dashboardService = dashboardService;
            _enrollmentService = enrollmentService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _dashboardService.GetStudentDashboardAsync(userId!);

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Errors.FirstOrDefault();
                return View();
            }

            return View(result.Data);
        }
    }
}