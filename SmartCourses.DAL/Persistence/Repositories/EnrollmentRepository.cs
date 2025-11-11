using Microsoft.EntityFrameworkCore;
using SmartCourses.DAL.Contracts.Repositories;
using SmartCourses.DAL.Entities;
using SmartCourses.DAL.Persistence.Data;

namespace SmartCourses.DAL.Persistence.Repositories
{
        public class EnrollmentRepository : GenericRepository<Enrollment, int>, IEnrollmentRepository
        {
            public EnrollmentRepository(ApplicationDbContext context) : base(context)
            {
            }

            public async Task<Enrollment?> GetEnrollmentAsync(string userId, int courseId)
            {
                return await _dbSet
                    .Include(e => e.Course)
                        .ThenInclude(c => c.Instructor)
                    .Include(e => e.Course)
                        .ThenInclude(c => c.Category)
                    .Include(e => e.Course.Sections)
                        .ThenInclude(s => s.Lessons)
                    .Include(e => e.LessonProgresses)
                        .ThenInclude(lp => lp.Lesson)
                    .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);
            }

            public async Task<IEnumerable<Enrollment>> GetUserEnrollmentsAsync(string userId)
            {
                return await _dbSet
                    .Where(e => e.UserId == userId)
                    .Include(e => e.Course)
                        .ThenInclude(c => c.Category)
                    .Include(e => e.Course)
                        .ThenInclude(c => c.Instructor)
                    .OrderByDescending(e => e.EnrolledAt)
                    .ToListAsync();
            }

            public async Task<IEnumerable<Enrollment>> GetCourseEnrollmentsAsync(int courseId)
            {
                return await _dbSet
                    .Where(e => e.CourseId == courseId)
                    .Include(e => e.User)
                    .OrderByDescending(e => e.EnrolledAt)
                    .ToListAsync();
            }

            public async Task<IEnumerable<Enrollment>> GetCompletedEnrollmentsAsync(string userId)
            {
                return await _dbSet
                    .Where(e => e.UserId == userId && e.IsCompleted)
                    .Include(e => e.Course)
                        .ThenInclude(c => c.Category)
                    .Include(e => e.Course)
                        .ThenInclude(c => c.Instructor)
                    .OrderByDescending(e => e.CompletedAt)
                    .ToListAsync();
            }

            public async Task<IEnumerable<Enrollment>> GetInProgressEnrollmentsAsync(string userId)
            {
                return await _dbSet
                    .Where(e => e.UserId == userId && !e.IsCompleted)
                    .Include(e => e.Course)
                        .ThenInclude(c => c.Category)
                    .Include(e => e.Course)
                        .ThenInclude(c => c.Instructor)
                    .OrderByDescending(e => e.LastModifiedOn)
                    .ToListAsync();
            }

            public async Task<bool> IsUserEnrolledAsync(string userId, int courseId)
            {
                return await _dbSet.AnyAsync(e => e.UserId == userId && e.CourseId == courseId);
            }

            public async Task<Enrollment?> GetEnrollmentWithProgressAsync(int enrollmentId)
            {
                return await _dbSet
                    .Include(e => e.Course)
                        .ThenInclude(c => c.Sections.OrderBy(s => s.Order))
                            .ThenInclude(s => s.Lessons.OrderBy(l => l.Order))
                    .Include(e => e.LessonProgresses)
                        .ThenInclude(lp => lp.Lesson)
                    .FirstOrDefaultAsync(e => e.Id == enrollmentId);
            }

            public async Task<int> GetCourseEnrollmentCountAsync(int courseId)
            {
                return await _dbSet.CountAsync(e => e.CourseId == courseId);
            }
        public async Task<IEnumerable<Enrollment>> GetRecentEnrollmentsAsync(int count = 10)
        {
            return await _dbSet
                .Include(e => e.User)
                .Include(e => e.Course)
                    .ThenInclude(c => c.Category)
                .OrderByDescending(e => e.EnrolledAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<int> GetEnrollmentCountByCourseIdsAsync(List<int> courseIds)
        {
            return await _dbSet.CountAsync(e => courseIds.Contains(e.CourseId));
        }
    }

    }

