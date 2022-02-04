using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Abstractions.Interfaces
{
    public interface IDbGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> Get(Expression<Func<T, bool>> filter,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
            string includeProperties);

        Task<T> GetByID(object id);

        Task Delete(object id);

        Task Delete(T entity);

        Task Update(T entityToUpdate);

        Task Create(T entity);

        IEnumerable<T> FindByCondition(Expression<Func<T, bool>> expression);

        T FirstOrDefault(Expression<Func<T, bool>> expression);

        bool Any();

        bool Any(Expression<Func<T, bool>> expression);
    }
}
