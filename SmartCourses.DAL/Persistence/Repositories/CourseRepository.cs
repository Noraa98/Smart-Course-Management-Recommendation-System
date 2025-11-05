using Microsoft.EntityFrameworkCore;
using SmartCourses.DAL.Contracts.Repositories;
using SmartCourses.DAL.Entities;
using SmartCourses.DAL.Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCourses.DAL.Persistence.Repositories
{
    public class CourseRepository : GenericRepository<Course>, ICourseRepository
    {
        public CourseRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Course>> GetCoursesWithSkillsAsync()
        {
            return await _dbSet
                .Include(c => c.Category)
                .Include(c => c.CourseSkills).ThenInclude(cs => cs.Skill)
                .ToListAsync();
        }

        public async Task<Course?> GetCourseWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(c => c.Category)
                .Include(c => c.CourseSkills).ThenInclude(cs => cs.Skill)
                .Include(c => c.Sections).ThenInclude(s => s.Lessons)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}