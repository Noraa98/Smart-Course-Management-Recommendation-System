using Microsoft.EntityFrameworkCore;
using SmartCourses.DAL.Contracts.Repositories;
using SmartCourses.DAL.Entities.RelationshipsTables;
using SmartCourses.DAL.Persistence.Data;

namespace SmartCourses.DAL.Persistence.Repositories
{
    public class UserSkillRepository : GenericRepository<UserSkill, (string, int)>, IUserSkillRepository
    {
        public UserSkillRepository(ApplicationDbContext context) : base(context) { }

        public async Task<UserSkill?> GetByKeysAsync(string userId, int skillId)
        {
            return await _dbSet
                .Include(us => us.Skill)
                .FirstOrDefaultAsync(us => us.UserId == userId && us.SkillId == skillId);
        }

        public async Task<IEnumerable<UserSkill>> GetUserSkillsAsync(string userId)
        {
            return await _dbSet
                .Where(us => us.UserId == userId)
                .Include(us => us.Skill)
                .ToListAsync();
        }
    }
}
