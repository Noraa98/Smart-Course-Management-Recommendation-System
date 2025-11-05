using Microsoft.EntityFrameworkCore;
using SmartCourses.DAL.Common.Enums;
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
    public class CourseRepository : GenericRepository<Course, int>, ICourseRepository
    {
        public CourseRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Course>> GetPublishedCoursesAsync()
        {
            return await _dbSet
                .Where(c => c.IsPublished)
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .Include(c => c.CourseSkills)
                    .ThenInclude(cs => cs.Skill)
                .OrderByDescending(c => c.CreatedOn)
                .ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetCoursesByInstructorAsync(string instructorId)
        {
            return await _dbSet
                .Where(c => c.InstructorId == instructorId)
                .Include(c => c.Category)
                .Include(c => c.Sections)
                .OrderByDescending(c => c.CreatedOn)
                .ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetCoursesByCategoryAsync(int categoryId)
        {
            return await _dbSet
                .Where(c => c.CategoryId == categoryId && c.IsPublished)
                .Include(c => c.Instructor)
                .Include(c => c.CourseSkills)
                    .ThenInclude(cs => cs.Skill)
                .ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetCoursesByLevelAsync(CourseLevel level)
        {
            return await _dbSet
                .Where(c => c.Level == level && c.IsPublished)
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetCoursesBySkillAsync(int skillId)
        {
            return await _dbSet
                .Where(c => c.CourseSkills.Any(cs => cs.SkillId == skillId) && c.IsPublished)
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .Include(c => c.CourseSkills)
                    .ThenInclude(cs => cs.Skill)
                .ToListAsync();
        }

        public async Task<IEnumerable<Course>> SearchCoursesAsync(string searchTerm)
        {
            var lowerSearchTerm = searchTerm.ToLower();

            return await _dbSet
                .Where(c => c.IsPublished &&
                           (c.Title.ToLower().Contains(lowerSearchTerm) ||
                            c.Description.ToLower().Contains(lowerSearchTerm) ||
                            c.ShortDescription.ToLower().Contains(lowerSearchTerm)))
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .ToListAsync();
        }

        public async Task<Course?> GetCourseWithDetailsAsync(int courseId)
        {
            return await _dbSet
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .Include(c => c.Sections.OrderBy(s => s.Order))
                    .ThenInclude(s => s.Lessons.OrderBy(l => l.Order))
                .Include(c => c.CourseSkills)
                    .ThenInclude(cs => cs.Skill)
                .Include(c => c.Reviews)
                    .ThenInclude(r => r.User)
                .Include(c => c.Enrollments)
                .FirstOrDefaultAsync(c => c.Id == courseId);
        }

        public async Task<IEnumerable<Course>> GetTopRatedCoursesAsync(int count = 10)
        {
            return await _dbSet
                .Where(c => c.IsPublished && c.Reviews.Any())
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .Include(c => c.Reviews)
                .OrderByDescending(c => c.Reviews.Average(r => r.Rating))
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetMostEnrolledCoursesAsync(int count = 10)
        {
            return await _dbSet
                .Where(c => c.IsPublished)
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .Include(c => c.Enrollments)
                .OrderByDescending(c => c.Enrollments.Count)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetRecentCoursesAsync(int count = 10)
        {
            return await _dbSet
                .Where(c => c.IsPublished)
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .OrderByDescending(c => c.CreatedOn)
                .Take(count)
                .ToListAsync();
        }
    }
}