using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCourses.BLL.Models.DTOs.CourseDTOs;
using SmartCourses.BLL.Models.DTOs.Response_ResultDTOs;
using SmartCourses.BLL.Services.Contracts;
using SmartCourses.DAL.Contracts;
using AutoMapper;

namespace SmartCourses.PL.Controllers.AdminArea
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CoursesController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CoursesController> _logger;

        public CoursesController(
            ICourseService courseService,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CoursesController> logger)
        {
            _courseService = courseService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        // List All Courses (including unpublished)
        [HttpGet]
        public async Task<IActionResult> Index(string? searchTerm = null, int pageNumber = 1, int pageSize = 12)
        {
            try
            {
                // Use GetPagedAsync with filter, but we'll modify the approach to get all courses
                // For admin, we want to see all courses including unpublished
                var filter = new CourseFilterDto
                {
                    SearchTerm = searchTerm,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    SortBy = "created",
                    SortOrder = "desc"
                };

                // Get all courses using UnitOfWork (including unpublished and deleted)
                var allCourses = (await _unitOfWork.Courses.GetAllAsync(
                    c => c.Category,
                    c => c.Instructor,
                    c => c.CourseSkills,
                    c => c.Enrollments,
                    c => c.Reviews)).ToList();

                // Filter by search term if provided
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    allCourses = allCourses.Where(c =>
                        (c.Title?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (c.Description?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)).ToList();
                }

                // Order by created date descending
                allCourses = allCourses.OrderByDescending(c => c.CreatedOn).ToList();

                // Get total count
                var totalCount = allCourses.Count;

                // Apply pagination
                var pagedCourses = allCourses
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // Map to DTOs
                var courseDtos = _mapper.Map<List<CourseListDto>>(pagedCourses);

                // Create paginated result
                var paginatedResult = new PaginatedResultDto<CourseListDto>(
                    courseDtos,
                    totalCount,
                    pageNumber,
                    pageSize);

                ViewBag.SearchTerm = searchTerm;
                return View(paginatedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading courses for admin");
                TempData["Error"] = "An error occurred while loading courses";
                return View(new PaginatedResultDto<CourseListDto>(new List<CourseListDto>(), 0, 1, pageSize));
            }
        }

        // Course Details
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var result = await _courseService.GetCourseWithDetailsAsync(id);

                if (!result.IsSuccess)
                {
                    TempData["Error"] = result.Errors.FirstOrDefault() ?? "Course not found";
                    return RedirectToAction(nameof(Index));
                }

                return View(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading course details for admin");
                TempData["Error"] = "An error occurred while loading course details";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}

