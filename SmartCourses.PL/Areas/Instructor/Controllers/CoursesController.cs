using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCourses.BLL.Models.DTOs.CourseDTOs;
using SmartCourses.BLL.Services.Contracts;
using SmartCourses.BLL.Services.Interfaces;
using System.Security.Claims;

namespace SmartCourses.PL.Areas.Instructor.InstructorArea
{
    [Area("Instructor")]
    [Authorize(Roles = "Instructor")]
    public class CoursesController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly ICategoryService _categoryService;
        private readonly ISkillService _skillService;
        private readonly IFileService _fileService;
        private readonly ILogger<CoursesController> _logger;

        public CoursesController(
            ICourseService courseService,
            ICategoryService categoryService,
            ISkillService skillService,
            IFileService fileService,
            ILogger<CoursesController> logger)
        {
            _courseService = courseService;
            _categoryService = categoryService;
            _skillService = skillService;
            _fileService = fileService;
            _logger = logger;
        }

     
        // My Courses
        [HttpGet]
        public async Task<IActionResult> Index()
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

		// Accept singular alias: /Instructor/Course -> redirect to /Instructor/Courses
		[HttpGet("/Instructor/Course")]
		public IActionResult CourseAlias()
		{
			return RedirectToAction(nameof(Index));
		}

   
        // Manage Course Content (Sections & Lessons)

        [HttpGet]
        public async Task<IActionResult> ManageContent(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _courseService.GetCourseWithDetailsAsync(id);

            if (!result.IsSuccess)
            {
                TempData["Error"] = "Course not found";
                return RedirectToAction(nameof(Index));
            }

            // Check ownership
            if (result.Data!.InstructorId != userId)
            {
                TempData["Error"] = "You are not authorized to manage this course";
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }


        // Add Section
        [HttpGet]
        public async Task<IActionResult> AddSection(int courseId)
        {
            var courseResult = await _courseService.GetByIdAsync(courseId);
            if (!courseResult.IsSuccess)
            {
                TempData["Error"] = "Course not found";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Course = courseResult.Data;
            return View(new SectionCreateDto { CourseId = courseId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSection(SectionCreateDto model)
        {
            if (!ModelState.IsValid)
            {
                var courseResult = await _courseService.GetByIdAsync(model.CourseId);
                ViewBag.Course = courseResult.Data;
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _courseService.AddSectionAsync(model, userId!);

            if (result.IsSuccess)
            {
                TempData["Success"] = "Section added successfully";
                return RedirectToAction(nameof(ManageContent), new { id = model.CourseId });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            var course = await _courseService.GetByIdAsync(model.CourseId);
            ViewBag.Course = course.Data;
            return View(model);
        }

  
        // Add Lesson
        [HttpGet]
        public async Task<IActionResult> AddLesson(int sectionId)
        {
            // You'll need to add a method to get section details
            ViewBag.SectionId = sectionId;
            return View(new LessonCreateDto { SectionId = sectionId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLesson(LessonCreateDto model, IFormFile? videoFile)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.SectionId = model.SectionId;
                return View(model);
            }

            // Handle file upload if provided
            if (videoFile != null && videoFile.Length > 0)
            {
                var validationResult = await _fileService.ValidateFileAsync(
                    videoFile,
                    new[] { ".mp4", ".avi", ".mov" },
                    100 * 1024 * 1024); // 100MB

                if (validationResult.IsSuccess)
                {
                    var uploadResult = await _fileService.UploadFileAsync(videoFile, "lessons");
                    if (uploadResult.IsSuccess)
                    {
                        model.ContentPath = uploadResult.Data;
                    }
                }
                else
                {
                    ModelState.AddModelError(nameof(videoFile), validationResult.Errors.FirstOrDefault() ?? "Invalid file");
                    ViewBag.SectionId = model.SectionId;
                    return View(model);
                }
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _courseService.AddLessonAsync(model, userId!);

            if (result.IsSuccess)
            {
                TempData["Success"] = "Lesson added successfully";
                // Need to get courseId from section
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            ViewBag.SectionId = model.SectionId;
            return View(model);
        }

 
        // Delete Section
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSection(int sectionId, int courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _courseService.DeleteSectionAsync(sectionId, userId!);

            if (result.IsSuccess)
            {
                TempData["Success"] = "Section deleted successfully";
            }
            else
            {
                TempData["Error"] = result.Errors.FirstOrDefault();
            }

            return RedirectToAction(nameof(ManageContent), new { id = courseId });
        }

        
        // Delete Lesson
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLesson(int lessonId, int courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _courseService.DeleteLessonAsync(lessonId, userId!);

            if (result.IsSuccess)
            {
                TempData["Success"] = "Lesson deleted successfully";
            }
            else
            {
                TempData["Error"] = result.Errors.FirstOrDefault();
            }

            return RedirectToAction(nameof(ManageContent), new { id = courseId });
        }

        
        // Course Students (Enrollments)

        [HttpGet]
        public async Task<IActionResult> Students(int courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var courseResult = await _courseService.GetByIdAsync(courseId);

            if (!courseResult.IsSuccess || courseResult.Data!.InstructorId != userId)
            {
                TempData["Error"] = "You are not authorized to view this";
                return RedirectToAction(nameof(Index));
            }

            // You'll need to inject IEnrollmentService
            ViewBag.Course = courseResult.Data;
            return View();
        }
    }
}
