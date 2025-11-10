using SmartCourses.DAL.Common.Enums;
using SmartCourses.DAL.Entities;

namespace SmartCourses.DAL.Contracts.Repositories
{
    public interface ICourseRepository : IGenericRepository<Course, int>
    {
        
        Task<IEnumerable<Course>> GetPublishedCoursesAsync();

        
        Task<IEnumerable<Course>> GetCoursesByInstructorAsync(string instructorId);

      
        Task<IEnumerable<Course>> GetCoursesByCategoryAsync(int categoryId);

      
        Task<IEnumerable<Course>> GetCoursesByLevelAsync(CourseLevel level);

        
        Task<IEnumerable<Course>> GetCoursesBySkillAsync(int skillId);

        
        Task<IEnumerable<Course>> SearchCoursesAsync(string searchTerm);

  
        Task<Course?> GetCourseWithDetailsAsync(int courseId);

        
        Task<IEnumerable<Course>> GetTopRatedCoursesAsync(int count = 10);

        
        Task<IEnumerable<Course>> GetMostEnrolledCoursesAsync(int count = 10);

       
        Task<IEnumerable<Course>> GetRecentCoursesAsync(int count = 10);
    }
}