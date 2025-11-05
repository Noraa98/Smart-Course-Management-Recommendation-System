using SmartCourses.DAL.Contracts.Repositories;
using SmartCourses.DAL.Entities.RelationshipsTables;
using SmartCourses.DAL.Persistence.Data;

namespace SmartCourses.DAL.Persistence.Repositories
{
    public class CourseSkillRepository : GenericRepository<CourseSkill>, ICourseSkillRepository
    {
        public CourseSkillRepository(ApplicationDbContext context) : base(context) { }
    }
    
}
