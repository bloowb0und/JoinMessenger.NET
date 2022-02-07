using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Abstractions.Interfaces
{
    public interface IDbGenericRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> Get(Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            string includeProperties);

        Task<TEntity> GetById(object id);

        Task DeleteById(object id);

        void Delete(TEntity entity);
        
        Task CreateAsync(TEntity entity);

        void Update(TEntity entityToUpdate);

        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> expression = null);

        Task<bool> Any(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "");
    }
}
