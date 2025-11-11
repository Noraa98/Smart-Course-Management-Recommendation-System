using SmartCourses.DAL.Entities.RelationshipsTables;

namespace SmartCourses.DAL.Contracts.Repositories
{
    public interface IUserSkillRepository : IGenericRepository<UserSkill, (string UserId, int SkillId)>
    {
        Task<UserSkill?> GetByKeysAsync(string userId, int skillId);
        Task<IEnumerable<UserSkill>> GetUserSkillsAsync(string userId);
    }
}
