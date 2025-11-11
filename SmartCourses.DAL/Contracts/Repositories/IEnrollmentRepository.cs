using SmartCourses.DAL.Entities;

namespace SmartCourses.DAL.Contracts.Repositories
{
    public interface IEnrollmentRepository : IGenericRepository<Enrollment, int>
    {
        Task<Enrollment?> GetEnrollmentAsync(string userId, int courseId);

        Task<IEnumerable<Enrollment>> GetUserEnrollmentsAsync(string userId);

       
        Task<IEnumerable<Enrollment>> GetCourseEnrollmentsAsync(int courseId);

    
        Task<IEnumerable<Enrollment>> GetCompletedEnrollmentsAsync(string userId);

        
        Task<IEnumerable<Enrollment>> GetInProgressEnrollmentsAsync(string userId);

        
        Task<bool> IsUserEnrolledAsync(string userId, int courseId);

       
        Task<Enrollment?> GetEnrollmentWithProgressAsync(int enrollmentId);

       
        Task<int> GetCourseEnrollmentCountAsync(int courseId);

        Task<IEnumerable<Enrollment>> GetRecentEnrollmentsAsync(int count = 10);
        Task<int> GetEnrollmentCountByCourseIdsAsync(List<int> courseIds);
    }
}
