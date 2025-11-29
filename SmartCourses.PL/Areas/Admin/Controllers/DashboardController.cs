using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCourses.BLL.Services.Interfaces;

namespace SmartCourses.PL.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(
            IDashboardService dashboardService,
            ILogger<DashboardController> logger)
        {
            _dashboardService = dashboardService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _dashboardService.GetAdminDashboardAsync();

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Errors.FirstOrDefault();
                return View();
            }

            return View(result.Data);
        }
    }
}
