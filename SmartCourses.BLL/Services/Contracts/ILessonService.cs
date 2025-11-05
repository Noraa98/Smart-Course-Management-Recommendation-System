using SmartCourses.DAL.Entities;

namespace SmartCourses.BLL.Services.Contracts
{
    public interface ILessonService : IGenericService<Lesson>
    {
        Task<IEnumerable<Lesson>> GetLessonsBySectionIdAsync(int sectionId);
    }
}