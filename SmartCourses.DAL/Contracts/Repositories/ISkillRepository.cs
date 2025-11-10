using SmartCourses.DAL.Entities;

namespace SmartCourses.DAL.Contracts.Repositories
{
    public interface ISkillRepository : IGenericRepository<Skill, int>
    {
        Task<IEnumerable<Skill>> GetSkillsWithCoursesAsync();
        Task<IEnumerable<Skill>> GetCourseSkillsAsync(int courseId);
        Task<IEnumerable<Skill>> GetUserSkillsAsync(string userId);
        Task<IEnumerable<Skill>> GetPopularSkillsAsync(int count = 10);
    }
}
