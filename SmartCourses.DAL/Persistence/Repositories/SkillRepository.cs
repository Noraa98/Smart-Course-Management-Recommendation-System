using Microsoft.EntityFrameworkCore;
using SmartCourses.DAL.Contracts.Repositories;
using SmartCourses.DAL.Entities;
using SmartCourses.DAL.Persistence.Data;

namespace SmartCourses.DAL.Persistence.Repositories
{
    public class SkillRepository : GenericRepository<Skill, int>, ISkillRepository
    {
        public SkillRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Skill>> GetSkillsWithCoursesAsync()
        {
            return await _dbSet
                .Include(s => s.CourseSkills)
                    .ThenInclude(cs => cs.Course)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Skill>> GetCourseSkillsAsync(int courseId)
        {
            return await _dbSet
                .Where(s => s.CourseSkills.Any(cs => cs.CourseId == courseId))
                .ToListAsync();
        }

        public async Task<IEnumerable<Skill>> GetUserSkillsAsync(string userId)
        {
            return await _dbSet
                .Where(s => s.UserSkills.Any(us => us.UserId == userId))
                .ToListAsync();
        }

        public async Task<IEnumerable<Skill>> GetPopularSkillsAsync(int count = 10)
        {
            return await _dbSet
                .Include(s => s.CourseSkills)
                .OrderByDescending(s => s.CourseSkills.Count)
                .Take(count)
                .ToListAsync();
        }
    }
}
