using Core.Models;
using DAL.Abstractions.Interfaces;
using DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class DbGenericRepository<TEntity> : IDbGenericRepository<TEntity> where TEntity : class
    {
        private readonly AppDbContext _context;
        private DbSet<TEntity> _dbSet;
        private UnitOfWork _unitOfWork;

        public DbGenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
            _unitOfWork = new UnitOfWork(_context);
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
                         (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }
            else
            {
                return await query.ToListAsync();
            }
        }

        public virtual async Task<TEntity> GetByID(object id)
        {  
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task Delete(object id)
        {
            if (id == null)
            {
                return;
            }

            TEntity entityToDelete = await _dbSet.FindAsync(id);
            await Delete(entityToDelete);
        }

        public async virtual Task Delete(TEntity entityToDelete)
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
            await _unitOfWork.Save();
        }

        public async virtual Task Update(TEntity entityToUpdate)
        {
            if (entityToUpdate == null)
            {
                return;
            }

            _dbSet.Attach(entityToUpdate);
            _context.Entry(entityToUpdate).State = EntityState.Modified;
            await _unitOfWork.Save();
        }

        public IEnumerable<TEntity> FindByCondition(Expression<Func<TEntity, bool>> expression)
        {
            return _dbSet.Where(expression.Compile());
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> expression)
        {
            return _dbSet.FirstOrDefault(expression.Compile());
        }

        public bool Any()
        {
            return _dbSet.Any();
        }

        public bool Any(Expression<Func<TEntity, bool>> expression)
        {
            try
            {
               _dbSet.Any(expression.Compile());
            }
            catch (NullReferenceException)
            {
                return false;
            }

            return _dbSet.Any(expression.Compile());
        }

        public async Task Create(TEntity entity)
        {
            _dbSet.Add(entity);
            await _unitOfWork.Save();
        }
    }
}
