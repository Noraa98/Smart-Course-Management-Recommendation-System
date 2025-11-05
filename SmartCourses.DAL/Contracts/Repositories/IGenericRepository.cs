using System.Linq.Expressions;

public interface IGenericRepository<TEntity, TKey>
        where TEntity : class
        where TKey : IEquatable<TKey>
{
    // Query Methods
    Task<TEntity?> GetByIdAsync(TKey id);

    
    Task<TEntity?> GetByIdAsync(TKey id, params Expression<Func<TEntity, object>>[] includes);

    
    Task<IEnumerable<TEntity>> GetAllAsync();

   
    Task<IEnumerable<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] includes);

    
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);

   
    Task<IEnumerable<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        params Expression<Func<TEntity, object>>[] includes);

   
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

    
    Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        params Expression<Func<TEntity, object>>[] includes);

   
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);

    
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null);

   
    // Command Methods
    Task<TEntity> AddAsync(TEntity entity);

    Task AddRangeAsync(IEnumerable<TEntity> entities);

 
    void Update(TEntity entity);

    void UpdateRange(IEnumerable<TEntity> entities);

    void Delete(TEntity entity);

    void DeleteRange(IEnumerable<TEntity> entities);

    void SoftDelete(TEntity entity);

    
    void SoftDeleteRange(IEnumerable<TEntity> entities);

   
    // Pagination Methods
    Task<(IEnumerable<TEntity> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        params Expression<Func<TEntity, object>>[] includes);
}