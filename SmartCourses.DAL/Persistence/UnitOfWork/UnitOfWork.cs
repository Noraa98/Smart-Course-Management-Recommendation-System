using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SmartCourses.DAL.Contracts;
using SmartCourses.DAL.Contracts.Repositories;
using SmartCourses.DAL.Persistence.Data;
using SmartCourses.DAL.Persistence.Repositories;
using System.Collections;

namespace SmartCourses.DAL.Persistence.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        // Lazy initialization for repositories
        private ICourseRepository? _courses;
        private IEnrollmentRepository? _enrollments;
        private IReviewRepository? _reviews;
        private ICategoryRepository? _categories;
        private ISkillRepository? _skills;
        private ILessonProgressRepository? _lessonProgresses;

        // Generic repository cache
        private readonly Dictionary<Type, object> _repositories;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _repositories = new Dictionary<Type, object>();
        }

        // Repository Properties - Lazy Loading

        public ICourseRepository Courses
        {
            get
            {
                if (_courses == null)
                {
                    _courses = new CourseRepository(_context);
                }
                return _courses;
            }
        }

        public IEnrollmentRepository Enrollments
        {
            get
            {
                if (_enrollments == null)
                {
                    _enrollments = new EnrollmentRepository(_context);
                }
                return _enrollments;
            }
        }

        public IReviewRepository Reviews
        {
            get
            {
                if (_reviews == null)
                {
                    _reviews = new ReviewRepository(_context);
                }
                return _reviews;
            }
        }

        public ICategoryRepository Categories
        {
            get
            {
                if (_categories == null)
                {
                    _categories = new CategoryRepository(_context);
                }
                return _categories;
            }
        }

        public ISkillRepository Skills
        {
            get
            {
                if (_skills == null)
                {
                    _skills = new SkillRepository(_context);
                }
                return _skills;
            }
        }

        public ILessonProgressRepository LessonProgresses
        {
            get
            {
                if (_lessonProgresses == null)
                {
                    _lessonProgresses = new LessonProgressRepository(_context);
                }
                return _lessonProgresses;
            }
        }

        
        // Generic Repository Access
        public IGenericRepository<TEntity, TKey> Repository<TEntity, TKey>()
            where TEntity : class
            where TKey : IEquatable<TKey>
        {
            var type = typeof(TEntity);

            if (_repositories.TryGetValue(type, out var existingRepository))
            {
                return (IGenericRepository<TEntity, TKey>)existingRepository;
            }

            var repositoryInstance = new GenericRepository<TEntity, TKey>(_context);
            _repositories[type] = repositoryInstance;

            return repositoryInstance;
        }

        // Transaction Methods
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public async Task BeginTransactionAsync()
        {
            if (_transaction != null)
            {
                throw new InvalidOperationException("A transaction is already in progress.");
            }

            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No transaction in progress to commit.");
            }

            try
            {
                await _context.SaveChangesAsync();
                await _transaction.CommitAsync();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No transaction in progress to rollback.");
            }

            try
            {
                await _transaction.RollbackAsync();
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        // Dispose Pattern
        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose transaction if exists
                    _transaction?.Dispose();

                    // Dispose context
                    _context.Dispose();

                    // Clear repository cache
                    _repositories.Clear();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~UnitOfWork()
        {
            Dispose(false);
        }
    }
}

