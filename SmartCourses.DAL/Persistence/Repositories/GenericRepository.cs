using Microsoft.EntityFrameworkCore;
using SmartCourses.DAL.Common.Entities;
using SmartCourses.DAL.Persistence.Data;
using System.Linq.Expressions;

namespace SmartCourses.DAL.Persistence.Repositories
{
    public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey>
        where TEntity : class
        where TKey : IEquatable<TKey>
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        // Query Methods

        public virtual async Task<TEntity?> GetByIdAsync(TKey id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<TEntity?> GetByIdAsync(TKey id, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet;
            query = includes.Aggregate(query, (current, include) => current.Include(include));

            // Build expression for Id comparison
            var parameter = Expression.Parameter(typeof(TEntity), "e");
            var property = Expression.Property(parameter, "Id");
            var constant = Expression.Constant(id);
            var equality = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<TEntity, bool>>(equality, parameter);

            return await query.FirstOrDefaultAsync(lambda);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet;
            query = includes.Aggregate(query, (current, include) => current.Include(include));
            return await query.ToListAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet;
            query = includes.Aggregate(query, (current, include) => current.Include(include));
            return await query.Where(predicate).ToListAsync();
        }

        public virtual async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public virtual async Task<TEntity?> FirstOrDefaultAsync(
            Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet;
            query = includes.Aggregate(query, (current, include) => current.Include(include));
            return await query.FirstOrDefaultAsync(predicate);
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null)
        {
            return predicate == null
                ? await _dbSet.CountAsync()
                : await _dbSet.CountAsync(predicate);
        }

      
        // Command Methods

        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public virtual void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public virtual void UpdateRange(IEnumerable<TEntity> entities)
        {
            _dbSet.UpdateRange(entities);
        }

        public virtual void Delete(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual void DeleteRange(IEnumerable<TEntity> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public virtual void SoftDelete(TEntity entity)
        {
            if (entity is BaseAuditableEntity<TKey> auditableEntity)
            {
                auditableEntity.IsDeleted = true;
                Update(entity);
            }
            else
            {
                Delete(entity);
            }
        }

        public virtual void SoftDeleteRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                SoftDelete(entity);
            }
        }

        // Pagination Methods
        public virtual async Task<(IEnumerable<TEntity> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<TEntity, bool>>? predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet;

            // Apply includes
            query = includes.Aggregate(query, (current, include) => current.Include(include));

            // Apply filter
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            // Get total count
            var totalCount = await query.CountAsync();

            // Apply ordering
            if (orderBy != null)
            {
                query = orderBy(query);
            }

            // Apply pagination
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}

