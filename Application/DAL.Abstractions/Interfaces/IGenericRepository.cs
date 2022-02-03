using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Models;

namespace DAL.Abstractions.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        IEnumerable<T> GetAll();
        
        IEnumerable<T> FindByCondition(Expression<Func<T, bool>> expression);

        T FirstOrDefault(Expression<Func<T, bool>> expression);
        
        T SingleOrDefault(Expression<Func<T, bool>> expression);

        bool Any();
        
        bool Any(Expression<Func<T, bool>> expression);

        Task CreateAsync(T entity);
        
        Task UpdateAsync(T entity);
        
        Task DeleteAsync(T entity);
    }
}