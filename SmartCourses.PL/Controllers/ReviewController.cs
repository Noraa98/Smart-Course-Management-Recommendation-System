using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCourses.BLL.Models.DTOs.Enrollment_ReviewDTOs;
using SmartCourses.BLL.Services.Contracts;
using System.Security.Claims;

namespace SmartCourses.PL.Controllers
{
    [Authorize(Roles = "Student")]
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly IEnrollmentService _enrollmentService;
        private readonly ICourseService _courseService;
        private readonly ILogger<ReviewController> _logger;

        public ReviewController(
            IReviewService reviewService,
            IEnrollmentService enrollmentService,
            ICourseService courseService,
            ILogger<ReviewController> logger)
        {
            _reviewService = reviewService;
            _enrollmentService = enrollmentService;
            _courseService = courseService;
            _logger = logger;
        }

 
        // My Reviews
        [HttpGet]
        public async Task<IActionResult> MyReviews()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _reviewService.GetUserReviewsAsync(userId!);

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Errors.FirstOrDefault();
                return View(new List<ReviewDto>());
            }

            return View(result.Data);
        }

        
        // Create Review

        [HttpGet]
        public async Task<IActionResult> Create(int courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if user is enrolled
            var enrollmentResult = await _enrollmentService.IsUserEnrolledAsync(userId!, courseId);
            if (!enrollmentResult.IsSuccess || !enrollmentResult.Data)
            {
                TempData["Error"] = "You must be enrolled in the course to leave a review";
                return RedirectToAction("Details", "Course", new { id = courseId });
            }

            // Check if already reviewed
            var hasReviewedResult = await _reviewService.HasUserReviewedCourseAsync(userId!, courseId);
            if (hasReviewedResult.IsSuccess && hasReviewedResult.Data)
            {
                TempData["Error"] = "You have already reviewed this course";
                return RedirectToAction("Details", "Course", new { id = courseId });
            }

            // Get course details
            var courseResult = await _courseService.GetByIdAsync(courseId);
            if (!courseResult.IsSuccess)
            {
                TempData["Error"] = "Course not found";
                return RedirectToAction("Index", "Course");
            }

            ViewBag.Course = courseResult.Data;

            return View(new ReviewCreateDto { CourseId = courseId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReviewCreateDto model)
        {
            if (!ModelState.IsValid)
            {
                var courseResult = await _courseService.GetByIdAsync(model.CourseId);
                ViewBag.Course = courseResult.Data;
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _reviewService.CreateAsync(model, userId!);

            if (result.IsSuccess)
            {
                TempData["Success"] = "Thank you for your review!";
                return RedirectToAction("Details", "Course", new { id = model.CourseId });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            var course = await _courseService.GetByIdAsync(model.CourseId);
            ViewBag.Course = course.Data;
            return View(model);
        }


        // Edit Review

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _reviewService.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                TempData["Error"] = "Review not found";
                return RedirectToAction(nameof(MyReviews));
            }

            // Check ownership
            if (result.Data!.UserId != userId)
            {
                TempData["Error"] = "You are not authorized to edit this review";
                return RedirectToAction(nameof(MyReviews));
            }

            var updateDto = new ReviewUpdateDto
            {
                Id = result.Data.Id,
                Rating = result.Data.Rating,
                Comment = result.Data.Comment
            };

            ViewBag.Review = result.Data;

            return View(updateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ReviewUpdateDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _reviewService.UpdateAsync(model, userId!);

            if (result.IsSuccess)
            {
                TempData["Success"] = "Review updated successfully";
                return RedirectToAction(nameof(MyReviews));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return View(model);
        }

        // Delete Review
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _reviewService.DeleteAsync(id, userId!);

            if (result.IsSuccess)
            {
                TempData["Success"] = "Review deleted successfully";
            }
            else
            {
                TempData["Error"] = result.Errors.FirstOrDefault();
            }

            return RedirectToAction(nameof(MyReviews));
        }

        
        // Quick Review (Modal/AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuickReview([FromBody] ReviewCreateDto model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data" });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _reviewService.CreateAsync(model, userId!);

            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "Thank you for your review!" });
            }

            return Json(new { success = false, message = result.Errors.FirstOrDefault() });
        }

        // Get Course Reviews (AJAX)
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCourseReviews(int courseId, int page = 1, int pageSize = 5)
        {
            var result = await _reviewService.GetCourseReviewsAsync(courseId);

            if (!result.IsSuccess)
            {
                return Json(new { success = false, message = result.Errors.FirstOrDefault() });
            }

            var reviews = result.Data!
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalCount = result.Data.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return Json(new
            {
                success = true,
                reviews = reviews,
                currentPage = page,
                totalPages = totalPages,
                hasMore = page < totalPages
            });
        }

        
        // Get Average Rating (AJAX)

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAverageRating(int courseId)
        {
            var result = await _reviewService.GetCourseAverageRatingAsync(courseId);

            if (result.IsSuccess)
            {
                return Json(new { success = true, averageRating = result.Data });
            }

            return Json(new { success = false, message = result.Errors.FirstOrDefault() });
        }
    }
}