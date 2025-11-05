using SmartCourses.DAL.Contracts.Repositories;
using SmartCourses.DAL.Entities.RelationshipsTables;
using SmartCourses.DAL.Persistence.Data;

namespace SmartCourses.DAL.Persistence.Repositories
{
    public class UserSkillRepository : GenericRepository<UserSkill>, IUserSkillRepository
    {
        public UserSkillRepository(ApplicationDbContext context) : base(context) { }
    }
}
