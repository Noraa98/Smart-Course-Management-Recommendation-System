using SmartCourses.DAL.Contracts.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCourses.DAL.Contracts
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        ICategoryRepository Categories { get; }
        ISkillRepository Skills { get; }
        ICourseRepository Courses { get; }
        ISectionRepository Sections { get; }
        ILessonRepository Lessons { get; }
        ILessonProgressRepository LessonProgress { get; }
        IEnrollmentRepository Enrollments { get; }
        IReviewRepository Reviews { get; }
        INotificationRepository Notifications { get; }
        IUserSkillRepository UserSkills { get; }
        ICourseSkillRepository CourseSkills { get; }

        Task<int> CompleteAsync();
    }
}