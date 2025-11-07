using Microsoft.AspNetCore.Mvc;
using SmartCourses.BLL.Models.DTOs.CourseDTOs;
using SmartCourses.BLL.Services.Contracts;
using SmartCourses.PL.ViewModels;
using System.Diagnostics;

namespace SmartCourses.PL.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            ICourseService courseService,
            ICategoryService categoryService,
            ILogger<HomeController> logger)
        {
            _courseService = courseService;
            _categoryService = categoryService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Get featured courses (most enrolled)
                var featuredCoursesResult = await _courseService.GetMostEnrolledCoursesAsync(6);

                // Get recent courses
                var recentCoursesResult = await _courseService.GetRecentCoursesAsync(6);

                // Get top rated courses
                var topRatedCoursesResult = await _courseService.GetTopRatedCoursesAsync(6);

                // Get categories
                var categoriesResult = await _categoryService.GetAllAsync();

                ViewBag.FeaturedCourses = featuredCoursesResult.IsSuccess ? featuredCoursesResult.Data : new List<CourseListDto>();
                ViewBag.RecentCourses = recentCoursesResult.IsSuccess ? recentCoursesResult.Data : new List<CourseListDto>();
                ViewBag.TopRatedCourses = topRatedCoursesResult.IsSuccess ? topRatedCoursesResult.Data : new List<CourseListDto>();
                ViewBag.Categories = categoriesResult.IsSuccess ? categoriesResult.Data : new List<BLL.Models.DTOs.CategoryDto>();

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading home page");
                return View();
            }
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
