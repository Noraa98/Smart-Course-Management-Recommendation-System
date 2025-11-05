using SmartCourses.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCourses.BLL.Services.Contracts
{
    public interface IEnrollmentService : IGenericService<Enrollment>
    {
        Task<bool> EnrollStudentAsync(string studentId, int courseId);
        Task<bool> UnenrollStudentAsync(string studentId, int courseId);
        Task<IEnumerable<Course>> GetEnrolledCoursesAsync(string studentId);
    }
}
