using System.Linq.Expressions;

namespace Tamrinak_API.Repository.GenericRepo
{
    public interface IGenericRepo<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> GetAsync(int id);
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> CreateAsync(TEntity entity);

        Task<TEntity> GetByConditionAsync(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> include = null);
        Task<TEntity> GetByConditionIncludeAsync(
             Expression<Func<TEntity, bool>> predicate,
             Func<IQueryable<TEntity>, IQueryable<TEntity>> include = null);


        Task<List<TEntity>> GetListByConditionAsync(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> include = null);

        Task<List<TEntity>> GetListByConditionIncludeAsync(
            Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> include = null);

        Task<bool> SaveAsync();
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
        Task AddAsync(TEntity entity);

    }
}
