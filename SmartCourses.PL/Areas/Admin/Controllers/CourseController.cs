using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartCourses.BLL.Models.DTOs.CourseDTOs;
using SmartCourses.BLL.Models.DTOs.Response_ResultDTOs;
using SmartCourses.BLL.Services.Contracts;
using SmartCourses.DAL.Contracts;
using SmartCourses.DAL.Entities.Identity;

namespace SmartCourses.PL.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CourseController : Controller
    {
            private readonly ICourseService _courseService;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;
            private readonly ILogger<CourseController> _logger;
            private readonly UserManager<ApplicationUser> _userManager;

            public CourseController(
                ICourseService courseService,
                IUnitOfWork unitOfWork,
                IMapper mapper,
                ILogger<CourseController> logger,
                UserManager<ApplicationUser> userManager)
            {
                _courseService = courseService;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _logger = logger;
                _userManager = userManager;
            }

            // List All Courses (including unpublished)
            [HttpGet]
            public async Task<IActionResult> Index(string? searchTerm = null, int pageNumber = 1, int pageSize = 12)
            {
                try
                {
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

            // GET: Create
            [HttpGet]
            public async Task<IActionResult> Create()
            {
                var categories = (await _unitOfWork.Categories.GetAllAsync()).ToList();
                var skills = (await _unitOfWork.Skills.GetAllAsync()).ToList();
                var instructors = await _userManager.GetUsersInRoleAsync("Instructor");

                ViewBag.Categories = categories.Select(c => new SelectListItem(c.Name, c.Id.ToString())).ToList();
                ViewBag.Skills = skills.Select(s => new SelectListItem(s.Name, s.Id.ToString())).ToList();
                ViewBag.Instructors = instructors.Select(i => new SelectListItem(i.UserName, i.Id)).ToList();

                return View(new CourseCreateDto());
            }

            // POST: Create
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create(CourseCreateDto model, string selectedInstructorId, List<int> skillIds)
            {
                if (!ModelState.IsValid)
                {
                    var categories = (await _unitOfWork.Categories.GetAllAsync()).ToList();
                    var skills = (await _unitOfWork.Skills.GetAllAsync()).ToList();
                    var instructors = await _userManager.GetUsersInRoleAsync("Instructor");

                    ViewBag.Categories = categories.Select(c => new SelectListItem(c.Name, c.Id.ToString())).ToList();
                    ViewBag.Skills = skills.Select(s => new SelectListItem(s.Name, s.Id.ToString())).ToList();
                    ViewBag.Instructors = instructors.Select(i => new SelectListItem(i.UserName, i.Id)).ToList();

                    TempData["Error"] = "Please correct the form errors.";
                    return View(model);
                }

                try
                {
                    if (string.IsNullOrWhiteSpace(selectedInstructorId))
                    {
                        TempData["Error"] = "Please select an instructor";
                        return await Create();
                    }

                    var result = await _courseService.CreateAsync(model, selectedInstructorId);

                    if (!result.IsSuccess)
                    {
                        TempData["Error"] = result.Errors.FirstOrDefault() ?? "Failed to create course";
                        return RedirectToAction(nameof(Index));
                    }

                    // Update skills if any
                    if (skillIds != null && skillIds.Any())
                    {
                        await _courseService.UpdateAsync(new CourseUpdateDto
                        {
                            Id = result.Data.Id,
                            Title = result.Data.Title,
                            Description = result.Data.Description,
                            ShortDescription = result.Data.ShortDescription,
                            ThumbnailPath = result.Data.ThumbnailPath,
                            Level = result.Data.Level == "Beginner" ? 1 : result.Data.Level == "Intermediate" ? 2 : 3,
                            Price = result.Data.Price,
                            DurationInHours = result.Data.DurationInHours,
                            CategoryId = result.Data.CategoryId,
                            IsPublished = result.Data.IsPublished,
                            SkillIds = skillIds
                        }, selectedInstructorId);
                    }

                    TempData["Success"] = "Course created successfully";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating course");
                    TempData["Error"] = "An error occurred while creating the course";
                    return RedirectToAction(nameof(Index));
                }
            }

            // GET: Edit
            [HttpGet]
            public async Task<IActionResult> Edit(int id)
            {
                var result = await _courseService.GetByIdAsync(id);
                if (!result.IsSuccess)
                {
                    TempData["Error"] = result.Errors.FirstOrDefault() ?? "Course not found";
                    return RedirectToAction(nameof(Index));
                }

                var course = result.Data;
                var updateDto = _mapper.Map<CourseUpdateDto>(course);

                var categories = (await _unitOfWork.Categories.GetAllAsync()).ToList();
                var skills = (await _unitOfWork.Skills.GetAllAsync()).ToList();
                var instructors = await _userManager.GetUsersInRoleAsync("Instructor");

                ViewBag.Categories = categories.Select(c => new SelectListItem(c.Name, c.Id.ToString(), c.Id == updateDto.CategoryId)).ToList();
                ViewBag.Skills = skills.Select(s => new SelectListItem(s.Name, s.Id.ToString())).ToList();
                ViewBag.Instructors = instructors.Select(i => new SelectListItem(i.UserName, i.Id, i.Id == updateDto.InstructorId)).ToList();

                return View(updateDto);
            }

            // POST: Edit
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(CourseUpdateDto model, string selectedInstructorId, List<int> skillIds)
            {
                if (!ModelState.IsValid)
                {
                    var categories = (await _unitOfWork.Categories.GetAllAsync()).ToList();
                    var skills = (await _unitOfWork.Skills.GetAllAsync()).ToList();
                    var instructors = await _userManager.GetUsersInRoleAsync("Instructor");

                    ViewBag.Categories = categories.Select(c => new SelectListItem(c.Name, c.Id.ToString(), c.Id == model.CategoryId)).ToList();
                    ViewBag.Skills = skills.Select(s => new SelectListItem(s.Name, s.Id.ToString())).ToList();
                    ViewBag.Instructors = instructors.Select(i => new SelectListItem(i.UserName, i.Id, i.Id == selectedInstructorId)).ToList();

                    TempData["Error"] = "Please correct the form errors.";
                    return View(model);
                }

                try
                {
                    // Use selected instructor if provided, otherwise keep current
                    if (!string.IsNullOrWhiteSpace(selectedInstructorId))
                    {
                        model.InstructorId = selectedInstructorId;
                    }

                    // Admin can modify any course; use admin id for LastModifiedBy
                    var adminId = (await _userManager.GetUserAsync(User))?.Id ?? string.Empty;
                    model.SkillIds = skillIds ?? new List<int>();

                    var result = await _courseService.UpdateAsync(model, adminId);

                    if (!result.IsSuccess)
                    {
                        TempData["Error"] = result.Errors.FirstOrDefault() ?? "Failed to update course";
                        return RedirectToAction(nameof(Index));
                    }

                    TempData["Success"] = "Course updated successfully";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating course");
                    TempData["Error"] = "An error occurred while updating the course";
                    return RedirectToAction(nameof(Index));
                }
            }

            // POST: Delete
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Delete(int id)
            {
                try
                {
                    var course = await _unitOfWork.Courses.GetByIdAsync(id);
                    if (course == null)
                    {
                        TempData["Error"] = "Course not found";
                        return RedirectToAction(nameof(Index));
                    }

                    // Soft delete directly as admin
                    _unitOfWork.Courses.SoftDelete(course);
                    await _unitOfWork.SaveChangesAsync();

                    TempData["Success"] = "Course deleted successfully";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error deleting course");
                    TempData["Error"] = "An error occurred while deleting the course";
                    return RedirectToAction(nameof(Index));
                }
            }
        }
    }
