using SmartCourses.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCourses.DAL.Contracts.Repositories
{
    public interface IReviewRepository : IGenericRepository<Review, int>
    {
        Task<IEnumerable<Review>> GetCourseReviewsAsync(int courseId);
        Task<IEnumerable<Review>> GetUserReviewsAsync(string userId);
        Task<Review?> GetUserCourseReviewAsync(string userId, int courseId);
        Task<double> GetCourseAverageRatingAsync(int courseId);
        Task<bool> HasUserReviewedCourseAsync(string userId, int courseId);
    }
}
