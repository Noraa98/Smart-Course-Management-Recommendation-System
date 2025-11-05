using SmartCourses.DAL.Entities;

namespace SmartCourses.BLL.Services.Contracts
{
    public interface ISectionService : IGenericService<Section>
    {
        Task<IEnumerable<Section>> GetSectionsByCourseIdAsync(int courseId);
    }
}