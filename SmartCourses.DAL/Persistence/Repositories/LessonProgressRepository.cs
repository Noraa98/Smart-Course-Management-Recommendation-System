using Microsoft.EntityFrameworkCore;
using SmartCourses.DAL.Contracts.Repositories;
using SmartCourses.DAL.Entities;
using SmartCourses.DAL.Persistence.Data;

namespace SmartCourses.DAL.Persistence.Repositories
{
    public class LessonProgressRepository : GenericRepository<LessonProgress, int>, ILessonProgressRepository
    {
        public LessonProgressRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<LessonProgress?> GetProgressAsync(int enrollmentId, int lessonId)
        {
            return await _dbSet
                .Include(lp => lp.Lesson)
                .FirstOrDefaultAsync(lp => lp.EnrollmentId == enrollmentId && lp.LessonId == lessonId);
        }

        public async Task<IEnumerable<LessonProgress>> GetEnrollmentProgressesAsync(int enrollmentId)
        {
            return await _dbSet
                .Where(lp => lp.EnrollmentId == enrollmentId)
                .Include(lp => lp.Lesson)
                .ToListAsync();
        }

        public async Task<int> GetCompletedLessonsCountAsync(int enrollmentId)
        {
            return await _dbSet
                .CountAsync(lp => lp.EnrollmentId == enrollmentId && lp.IsCompleted);
        }

        public async Task<decimal> CalculateEnrollmentProgressAsync(int enrollmentId)
        {
            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                    .ThenInclude(c => c.Sections)
                        .ThenInclude(s => s.Lessons)
                .FirstOrDefaultAsync(e => e.Id == enrollmentId);

            if (enrollment == null) return 0;

            var totalLessons = enrollment.Course.Sections
                .SelectMany(s => s.Lessons).Count();

            if (totalLessons == 0) return 0;

            var completedLessons = await GetCompletedLessonsCountAsync(enrollmentId);

            return Math.Round((decimal)completedLessons / totalLessons * 100, 2);
        }
    }
}
