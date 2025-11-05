using SmartCourses.DAL.Contracts.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCourses.DAL.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        // Repositories
        ICourseRepository Courses { get; }
        IEnrollmentRepository Enrollments { get; }
        IReviewRepository Reviews { get; }
        ICategoryRepository Categories { get; }
        ISkillRepository Skills { get; }
        ILessonProgressRepository LessonProgresses { get; }

        
        // Generic Repository Access
        IGenericRepository<TEntity, TKey> Repository<TEntity, TKey>()
            where TEntity : class
            where TKey : IEquatable<TKey>;

        
        // Transaction Methods
        
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        
        int SaveChanges();

       
        Task BeginTransactionAsync();

       
        Task CommitTransactionAsync();

       
        Task RollbackTransactionAsync();
    }
}