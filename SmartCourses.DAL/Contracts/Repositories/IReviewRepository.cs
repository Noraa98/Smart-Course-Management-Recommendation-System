using SmartCourses.DAL.Entities;

namespace SmartCourses.DAL.Contracts.Repositories
{
    public interface IReviewRepository : IGenericRepository<Review, int>
    {
        Task<IEnumerable<Review>> GetCourseReviewsAsync(int courseId);
        Task<IEnumerable<Review>> GetUserReviewsAsync(string userId);
        Task<Review?> GetUserCourseReviewAsync(string userId, int courseId);
        Task<double> GetCourseAverageRatingAsync(int courseId);
        Task<bool> HasUserReviewedCourseAsync(string userId, int courseId);
        Task<IEnumerable<Review>> GetReviewsByCourseIdsAsync(List<int> courseIds);
        Task<int> GetReviewCountByCourseIdsAsync(List<int> courseIds);
        Task<double> GetAverageRatingByCourseIdsAsync(List<int> courseIds);
    }
}
