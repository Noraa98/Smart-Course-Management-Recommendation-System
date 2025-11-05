using SmartCourses.DAL.Entities;

namespace SmartCourses.BLL.Services.Contracts
{
    public interface ISkillService : IGenericService<Skill>
    {
        Task<Skill?> GetByNameAsync(string name);
        Task<IEnumerable<Skill>> GetSkillsByCategoryIdAsync(int categoryId);
    }
}