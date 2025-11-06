using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCourses.BLL.Models.DTOs;
using SmartCourses.BLL.Services.Contracts;
using System.Security.Claims;

namespace SmartCourses.PL.Controllers.AdminArea
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(
            ICategoryService categoryService,
            ILogger<CategoriesController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }


        // List Categories
        [HttpGet]
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
        {
            var result = await _categoryService.GetPagedAsync(pageNumber, pageSize);

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Errors.FirstOrDefault();
                return View();
            }

            return View(result.Data);
        }

        // Create Category
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _categoryService.CreateAsync(model, userId!);

            if (result.IsSuccess)
            {
                TempData["Success"] = "Category created successfully";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return View(model);
        }


        // Edit Category
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _categoryService.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                TempData["Error"] = "Category not found";
                return RedirectToAction(nameof(Index));
            }

            var updateDto = new CategoryUpdateDto
            {
                Id = result.Data!.Id,
                Name = result.Data.Name,
                Description = result.Data.Description,
                IconPath = result.Data.IconPath
            };

            return View(updateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryUpdateDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _categoryService.UpdateAsync(model, userId!);

            if (result.IsSuccess)
            {
                TempData["Success"] = "Category updated successfully";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return View(model);
        }

      
        // Delete Category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoryService.DeleteAsync(id);

            if (result.IsSuccess)
            {
                TempData["Success"] = "Category deleted successfully";
            }
            else
            {
                TempData["Error"] = result.Errors.FirstOrDefault();
            }

            return RedirectToAction(nameof(Index));
        }


        // Details
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var result = await _categoryService.GetCategoryWithCoursesAsync(id);

            if (!result.IsSuccess)
            {
                TempData["Error"] = "Category not found";
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }
    }
}