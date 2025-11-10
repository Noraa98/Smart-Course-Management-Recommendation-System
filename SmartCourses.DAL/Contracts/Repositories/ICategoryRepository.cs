using SmartCourses.DAL.Entities;

namespace SmartCourses.DAL.Contracts.Repositories
{
    public interface ICategoryRepository : IGenericRepository<Category, int>
    {
        Task<Category?> GetCategoryWithCoursesAsync(int categoryId);
        Task<IEnumerable<Category>> GetCategoriesWithCourseCountAsync();
        Task<bool> CategoryHasCoursesAsync(int categoryId);
    }
}