using SmartCourses.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCourses.BLL.Services.Contracts
{
    public interface ICourseService : IGenericService<Course>
    {
        Task<IEnumerable<Course>> GetCoursesByInstructorIdAsync(string instructorId);
        Task<IEnumerable<Course>> GetCoursesByCategoryIdAsync(int categoryId);
        Task<IEnumerable<Course>> GetCoursesBySkillIdAsync(int skillId);
        Task PublishCourseAsync(int courseId);
        Task<IEnumerable<Course>> SearchCoursesAsync(string keyword);
    }
}
