using Microsoft.EntityFrameworkCore;
using SmartCourses.DAL.Contracts.Repositories;
using SmartCourses.DAL.Entities;
using SmartCourses.DAL.Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCourses.DAL.Persistence.Repositories
{
    public class CategoryRepository : GenericRepository<Category, int>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Category?> GetCategoryWithCoursesAsync(int categoryId)
        {
            return await _dbSet
                .Include(c => c.Courses.Where(course => course.IsPublished))
                    .ThenInclude(course => course.Instructor)
                .FirstOrDefaultAsync(c => c.Id == categoryId);
        }

        public async Task<IEnumerable<Category>> GetCategoriesWithCourseCountAsync()
        {
            return await _dbSet
                .Include(c => c.Courses)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<bool> CategoryHasCoursesAsync(int categoryId)
        {
            return await _context.Courses.AnyAsync(c => c.CategoryId == categoryId);
        }
    }
}
