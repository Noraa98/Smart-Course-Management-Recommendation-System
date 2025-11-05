using Microsoft.EntityFrameworkCore;
using SmartCourses.DAL.Contracts.Repositories;
using SmartCourses.DAL.Entities;
using SmartCourses.DAL.Persistence.Data;

namespace SmartCourses.DAL.Persistence.Repositories
{
    public class ReviewRepository : GenericRepository<Review, int>, IReviewRepository
    {
        public ReviewRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Review>> GetCourseReviewsAsync(int courseId)
        {
            return await _dbSet
                .Where(r => r.CourseId == courseId)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedOn)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetUserReviewsAsync(string userId)
        {
            return await _dbSet
                .Where(r => r.UserId == userId)
                .Include(r => r.Course)
                .OrderByDescending(r => r.CreatedOn)
                .ToListAsync();
        }

        public async Task<Review?> GetUserCourseReviewAsync(string userId, int courseId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(r => r.UserId == userId && r.CourseId == courseId);
        }

        public async Task<double> GetCourseAverageRatingAsync(int courseId)
        {
            var reviews = await _dbSet.Where(r => r.CourseId == courseId).ToListAsync();
            return reviews.Any() ? reviews.Average(r => r.Rating) : 0;
        }

        public async Task<bool> HasUserReviewedCourseAsync(string userId, int courseId)
        {
            return await _dbSet.AnyAsync(r => r.UserId == userId && r.CourseId == courseId);
        }
    }
}

