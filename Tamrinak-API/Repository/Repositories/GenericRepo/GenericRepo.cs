using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using Tamrinak_API.DataAccess;

namespace Tamrinak_API.Repository.Repositories.GenericRepo
{
    public class GenericRepo<TEntity> : IGenericRepo<TEntity> where TEntity : class
    {
        private readonly DatabaseContext _context;
        private DbSet<TEntity> _dbSet;
        public GenericRepo(DatabaseContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await SaveAsync();
        }

        public async Task<TEntity> CreateAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await SaveAsync();
            return entity;
        }

        public async Task DeleteAsync(TEntity entity)
        {
            _dbSet.Remove(entity);
            await SaveAsync();
        }

        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<TEntity> GetAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<TEntity> GetByConditionAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> include = null)
        {
            IQueryable<TEntity> query = _dbSet;
            if (include != null)
                query = query.Include(include);

            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<List<TEntity>> GetListByConditionAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> include = null)
        {
            IQueryable<TEntity> query = _dbSet;
            if (include != null)
                query = query.Include(include);

            return await query.Where(predicate).ToListAsync();
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity);
            await SaveAsync();
        }
    }
}
