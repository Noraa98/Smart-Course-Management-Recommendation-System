using SmartCourses.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCourses.DAL.Contracts.Repositories
{
    public interface ILessonProgressRepository : IGenericRepository<LessonProgress, int>
    {
        Task<LessonProgress?> GetProgressAsync(int enrollmentId, int lessonId);
        Task<IEnumerable<LessonProgress>> GetEnrollmentProgressesAsync(int enrollmentId);
        Task<int> GetCompletedLessonsCountAsync(int enrollmentId);
        Task<decimal> CalculateEnrollmentProgressAsync(int enrollmentId);
    }
}
