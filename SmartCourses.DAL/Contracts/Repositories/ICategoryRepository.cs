using SmartCourses.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCourses.DAL.Contracts.Repositories
{
    public interface ICategoryRepository : IGenericRepository<Category, int>
    {
        Task<Category?> GetCategoryWithCoursesAsync(int categoryId);
        Task<IEnumerable<Category>> GetCategoriesWithCourseCountAsync();
        Task<bool> CategoryHasCoursesAsync(int categoryId);
    }
}