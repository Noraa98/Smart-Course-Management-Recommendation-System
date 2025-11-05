using SmartCourses.DAL.Entities;

namespace SmartCourses.BLL.Services.Contracts
{
    public interface ICategoryService : IGenericService<Category>
    {
        Task<Category?> GetByNameAsync(string name);
        Task<Category?> GetCategoryWithCoursesAsync(int id);
    }
}
