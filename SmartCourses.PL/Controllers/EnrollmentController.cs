using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCourses.BLL.Models.DTOs.Enrollment_ReviewDTOs;
using SmartCourses.BLL.Services.Contracts;
using SmartCourses.PL.ViewModels;
using System.Security.Claims;

namespace SmartCourses.PL.Controllers
{
    [Authorize(Roles = "Student")]
    public class EnrollmentController : Controller
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly ICourseService _courseService;
        private readonly ILogger<EnrollmentController> _logger;

        public EnrollmentController(
            IEnrollmentService enrollmentService,
            ICourseService courseService,
            ILogger<EnrollmentController> logger)
        {
            _enrollmentService = enrollmentService;
            _courseService = courseService;
            _logger = logger;
        }

        
        // My Courses (All Enrollments)

        [HttpGet]
        public async Task<IActionResult> MyCourses()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _enrollmentService.GetUserEnrollmentsAsync(userId!);

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Errors.FirstOrDefault();
                return View(new List<EnrollmentDto>());
            }

            return View(result.Data);
        }

       
        // In Progress Courses
        [HttpGet]
        public async Task<IActionResult> InProgress()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _enrollmentService.GetInProgressEnrollmentsAsync(userId!);

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Errors.FirstOrDefault();
                return View(new List<EnrollmentDto>());
            }

            return View(result.Data);
        }

        // Completed Courses
        [HttpGet]
        public async Task<IActionResult> Completed()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _enrollmentService.GetCompletedEnrollmentsAsync(userId!);

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Errors.FirstOrDefault();
                return View(new List<EnrollmentDto>());
            }

            return View(result.Data);
        }

        // Enrollment Details (Course Player)
        [HttpGet]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Details(int id, int? lessonId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _enrollmentService.GetEnrollmentDetailsAsync(id, userId);

            if (!result.IsSuccess)
                return NotFound();

            var enrollment = result.Data;
            var allLessons = enrollment.Course.Sections
                  .OrderBy(s => s.Order)
                  .SelectMany(s => s.Lessons.OrderBy(l => l.Order))
                  .ToList();


            // Get current lesson
            var currentLesson = lessonId.HasValue
                ? allLessons.FirstOrDefault(l => l.Id == lessonId.Value)
                : allLessons.FirstOrDefault();

            if (currentLesson == null)
                return NotFound();

            // Get previous/next lessons
            var currentIndex = allLessons.IndexOf(currentLesson);
            var previousLesson = currentIndex > 0 ? allLessons[currentIndex - 1] : null;
            var nextLesson = currentIndex < allLessons.Count - 1 ? allLessons[currentIndex + 1] : null;

            // Get completed lessons
            var completedLessonIds = enrollment.LessonProgresses
                .Where(lp => lp.IsCompleted)
                .Select(lp => lp.LessonId)
                .ToList();

            var viewModel = new EnrollmentDetailsViewModel
            {
                EnrollmentId = enrollment.Id,
                Course = enrollment.Course,
                CurrentLesson = currentLesson,
                PreviousLessonId = previousLesson?.Id,
                NextLessonId = nextLesson?.Id,
                TotalLessons = allLessons.Count,
                CompletedLessons = completedLessonIds.Count,
                ProgressPercent = enrollment.ProgressPercent,
                CompletedLessonIds = completedLessonIds
            };

            return View(viewModel);
        }

        // Enroll in Course
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var enrollmentDto = new EnrollmentCreateDto
            {
                CourseId = courseId
            };

            var result = await _enrollmentService.EnrollAsync(enrollmentDto, userId!);

            if (result.IsSuccess)
            {
                TempData["Success"] = "You have successfully enrolled in this course!";
                return RedirectToAction("Details", "Course", new { id = courseId });
            }

            TempData["Error"] = result.Errors.FirstOrDefault();
            return RedirectToAction("Details", "Course", new { id = courseId });
        }

       
        // Unenroll from Course
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unenroll(int enrollmentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _enrollmentService.UnenrollAsync(enrollmentId, userId!);

            if (result.IsSuccess)
            {
                TempData["Success"] = "You have successfully unenrolled from this course.";
            }
            else
            {
                TempData["Error"] = result.Errors.FirstOrDefault();
            }

            return RedirectToAction(nameof(MyCourses));
        }

        // Update Lesson Progress (AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProgress([FromBody] LessonProgressUpdateDto progressDto)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data" });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _enrollmentService.UpdateLessonProgressAsync(progressDto, userId!);

            if (result.IsSuccess)
            {
                // Get updated progress percentage
                var progressResult = await _enrollmentService.CalculateProgressAsync(progressDto.EnrollmentId);

                return Json(new
                {
                    success = true,
                    message = result.Message,
                    progress = progressResult.IsSuccess ? progressResult.Data : 0
                });
            }

            return Json(new { success = false, message = result.Errors.FirstOrDefault() });
        }

     
        // Mark Lesson as Complete (AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkLessonComplete(int enrollmentId, int lessonId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var progressDto = new LessonProgressUpdateDto
            {
                EnrollmentId = enrollmentId,
                LessonId = lessonId,
                IsCompleted = true,
                WatchedSeconds = 0
            };

            var result = await _enrollmentService.UpdateLessonProgressAsync(progressDto, userId!);

            if (result.IsSuccess)
            {
                var progressResult = await _enrollmentService.CalculateProgressAsync(enrollmentId);

                return Json(new
                {
                    success = true,
                    message = "Lesson marked as complete",
                    progress = progressResult.IsSuccess ? progressResult.Data : 0
                });
            }

            return Json(new { success = false, message = result.Errors.FirstOrDefault() });
        }

        
        // Mark Course as Complete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkComplete(int enrollmentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _enrollmentService.MarkEnrollmentAsCompleteAsync(enrollmentId, userId!);

            if (result.IsSuccess)
            {
                TempData["Success"] = "Congratulations! You have completed this course.";
            }
            else
            {
                TempData["Error"] = result.Errors.FirstOrDefault();
            }

            return RedirectToAction(nameof(Details), new { id = enrollmentId });
        }

      
        // Get Enrollment Progress (AJAX)
        [HttpGet]
        public async Task<IActionResult> GetProgress(int enrollmentId)
        {
            var result = await _enrollmentService.CalculateProgressAsync(enrollmentId);

            if (result.IsSuccess)
            {
                return Json(new { success = true, progress = result.Data });
            }

            return Json(new { success = false, message = result.Errors.FirstOrDefault() });
        }
    }
}