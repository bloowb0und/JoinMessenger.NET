using DAL.Abstractions.Interfaces;
using DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class DbGenericRepository<TEntity> : IDbGenericRepository<TEntity> 
        where TEntity : class
    {
        private readonly AppDbContext _context;
        private DbSet<TEntity> _dbSet;

        public DbGenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public virtual async Task<IEnumerable<TEntity>> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                         (new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }

            return await query.ToListAsync();
        }

        public virtual async Task<TEntity> GetById(object id)
        {  
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task DeleteById(object id)
        {
            if (id == null)
            {
                return;
            }

            TEntity entityToDelete = await _dbSet.FindAsync(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (entityToDelete == null)
            {
                return;
            }

            if (_context.Entry(entityToDelete).State == EntityState.Detached)
            {
                _dbSet.Attach(entityToDelete);
            }

            _dbSet.Remove(entityToDelete);
        }
        
        public async Task CreateAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            if (entityToUpdate == null)
            {
                return;
            }

            _dbSet.Attach(entityToUpdate);
            _context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> expression = null)
        {
            return expression == null ? _dbSet.FirstOrDefault() : _dbSet.FirstOrDefault(expression.Compile());
        }

        public async Task<bool> Any(Expression<Func<TEntity, bool>> filter = null, 
            string includeProperties = "")
        {
            if (filter == null && string.IsNullOrWhiteSpace(includeProperties))
            {
                return _dbSet.Any();
            }
            
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                         (new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
            
            return (await query.ToListAsync()).Any();
        }
    }
}
