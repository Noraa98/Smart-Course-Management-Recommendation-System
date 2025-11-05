using SmartCourses.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
