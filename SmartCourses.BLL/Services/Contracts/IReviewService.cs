using SmartCourses.DAL.Entities;

namespace SmartCourses.BLL.Services.Contracts
{
    public interface IReviewService : IGenericService<Review>
    {
        Task<IEnumerable<Review>> GetReviewsByCourseIdAsync(int courseId);
        Task<double> GetAverageRatingAsync(int courseId);
    }
}