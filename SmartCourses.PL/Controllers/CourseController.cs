using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCourses.BLL.Models.DTOs.CourseDTOs;
using SmartCourses.BLL.Models.DTOs.Enrollment_ReviewDTOs;
using SmartCourses.BLL.Models.DTOs.Response_ResultDTOs;
using SmartCourses.BLL.Models.DTOs;
using SmartCourses.BLL.Services.Contracts;
using System.Security.Claims;

namespace SmartCourses.PL.Controllers
{
    public class CourseController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly ICategoryService _categoryService;
        private readonly ISkillService _skillService;
        private readonly IEnrollmentService _enrollmentService;
        private readonly IReviewService _reviewService;
        private readonly ILogger<CourseController> _logger;

        public CourseController(
            ICourseService courseService,
            ICategoryService categoryService,
            ISkillService skillService,
            IEnrollmentService enrollmentService,
            IReviewService reviewService,
            ILogger<CourseController> logger)
        {
            _courseService = courseService;
            _categoryService = categoryService;
            _skillService = skillService;
            _enrollmentService = enrollmentService;
            _reviewService = reviewService;
            _logger = logger;
        }

        
        // List Courses
        [HttpGet]
        public async Task<IActionResult> Index(
            string? searchTerm,
            int? categoryId,
            int? level,
            int? skillId,
            string? sortBy,
            int pageNumber = 1,
            int pageSize = 12)
        {
            var filter = new CourseFilterDto
            {
                SearchTerm = searchTerm,
                CategoryId = categoryId,
                Level = level,
                SkillId = skillId,
                SortBy = sortBy,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _courseService.GetPagedAsync(filter);

            // Get categories and skills for filters
            var categoriesResult = await _categoryService.GetAllAsync();
            var skillsResult = await _skillService.GetAllAsync();

            ViewBag.Categories = categoriesResult.IsSuccess ? categoriesResult.Data : new List<CategoryDto>();
            ViewBag.Skills = skillsResult.IsSuccess ? skillsResult.Data : new List<SkillDto>();
            ViewBag.Filter = filter;

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Errors.FirstOrDefault();
                return View(new PaginatedResultDto<CourseListDto>());
            }

            return View(result.Data);
        }

      
        // Course Details
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var result = await _courseService.GetCourseWithDetailsAsync(id);

            if (!result.IsSuccess)
            {
                TempData["Error"] = "Course not found";
                return RedirectToAction(nameof(Index));
            }

            // Check if user is enrolled
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var enrollmentResult = await _enrollmentService.IsUserEnrolledAsync(userId!, id);
                ViewBag.IsEnrolled = enrollmentResult.IsSuccess && enrollmentResult.Data;

                // Check if user has reviewed
                var hasReviewedResult = await _reviewService.HasUserReviewedCourseAsync(userId!, id);
                ViewBag.HasReviewed = hasReviewedResult.IsSuccess && hasReviewedResult.Data;
            }
            else
            {
                ViewBag.IsEnrolled = false;
                ViewBag.HasReviewed = false;
            }

            // Get course reviews
            var reviewsResult = await _reviewService.GetCourseReviewsAsync(id);
            ViewBag.Reviews = reviewsResult.IsSuccess ? reviewsResult.Data : new List<ReviewDto>();

            return View(result.Data);
        }

        // Search
        [HttpGet]
        public async Task<IActionResult> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return RedirectToAction(nameof(Index));
            }

            var result = await _courseService.SearchCoursesAsync(searchTerm);

            ViewBag.SearchTerm = searchTerm;

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Errors.FirstOrDefault();
                return View(new List<CourseListDto>());
            }

            return View(result.Data);
        }

       
        // By Category
        [HttpGet]
        public async Task<IActionResult> ByCategory(int categoryId)
        {
            var categoryResult = await _categoryService.GetByIdAsync(categoryId);
            if (!categoryResult.IsSuccess)
            {
                TempData["Error"] = "Category not found";
                return RedirectToAction(nameof(Index));
            }

            var result = await _courseService.GetCoursesByCategoryAsync(categoryId);

            ViewBag.Category = categoryResult.Data;

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Errors.FirstOrDefault();
                return View(new List<CourseListDto>());
            }

            return View(result.Data);
        }

        
        // Instructor Courses (Create, Edit, Delete)
        [HttpGet]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> MyCourses()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _courseService.GetInstructorCoursesAsync(userId!);

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Errors.FirstOrDefault();
                return View(new List<CourseListDto>());
            }

            return View(result.Data);
        }

        [HttpGet]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> Create()
        {
            await LoadCreateEditViewBagAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> Create(CourseCreateDto model)
        {
            if (!ModelState.IsValid)
            {
                await LoadCreateEditViewBagAsync();
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _courseService.CreateAsync(model, userId!);

            if (result.IsSuccess)
            {
                TempData["Success"] = "Course created successfully";
                return RedirectToAction(nameof(MyCourses));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            await LoadCreateEditViewBagAsync();
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _courseService.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                TempData["Error"] = "Course not found";
                return RedirectToAction(nameof(MyCourses));
            }

            // Check ownership
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (result.Data!.InstructorId != userId)
            {
                TempData["Error"] = "You are not authorized to edit this course";
                return RedirectToAction(nameof(MyCourses));
            }

            var updateDto = new CourseUpdateDto
            {
                Id = result.Data.Id,
                Title = result.Data.Title,
                Description = result.Data.Description,
                ShortDescription = result.Data.ShortDescription,
                ThumbnailPath = result.Data.ThumbnailPath,
                Level = (int)Enum.Parse(typeof(DAL.Common.Enums.CourseLevel), result.Data.Level),
                Price = result.Data.Price,
                DurationInHours = result.Data.DurationInHours,
                CategoryId = result.Data.CategoryId,
                IsPublished = result.Data.IsPublished,
                SkillIds = result.Data.Skills.Select(s => s.Id).ToList()
            };

            await LoadCreateEditViewBagAsync();
            return View(updateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> Edit(CourseUpdateDto model)
        {
            if (!ModelState.IsValid)
            {
                await LoadCreateEditViewBagAsync();
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _courseService.UpdateAsync(model, userId!);

            if (result.IsSuccess)
            {
                TempData["Success"] = "Course updated successfully";
                return RedirectToAction(nameof(Details), new { id = model.Id });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            await LoadCreateEditViewBagAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _courseService.DeleteAsync(id, userId!);

            if (result.IsSuccess)
            {
                TempData["Success"] = "Course deleted successfully";
            }
            else
            {
                TempData["Error"] = result.Errors.FirstOrDefault();
            }

            return RedirectToAction(nameof(MyCourses));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> Publish(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _courseService.PublishCourseAsync(id, userId!);

            if (result.IsSuccess)
            {
                TempData["Success"] = "Course published successfully";
            }
            else
            {
                TempData["Error"] = result.Errors.FirstOrDefault();
            }

            return RedirectToAction(nameof(Details), new { id });
        }

       
        // Helper Methods

        private async Task LoadCreateEditViewBagAsync()
        {
            var categoriesResult = await _categoryService.GetAllAsync();
            var skillsResult = await _skillService.GetAllAsync();

            ViewBag.Categories = categoriesResult.IsSuccess ? categoriesResult.Data : new List<CategoryDto>();
            ViewBag.Skills = skillsResult.IsSuccess ? skillsResult.Data : new List<SkillDto>();
        }
    }
}