using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Models;
using DAL.Abstractions.Interfaces;
using DAL.Contexts;

namespace DAL.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly MessengerContext _messengerContext;
        
        public GenericRepository(MessengerContext messengerContext)
        {
            _messengerContext = messengerContext;
        }

        public IEnumerable<T> GetAll()
        {
            return _messengerContext.GetSet<T>();
        }

        public IEnumerable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            var m = GetAll();
            return m.Where(expression.Compile());
        }
        
        public T FirstOrDefault(Expression<Func<T, bool>> expression)
        {
            var m = GetAll();
            return m.FirstOrDefault(expression.Compile());
        }
        
        public T SingleOrDefault(Expression<Func<T, bool>> expression)
        {
            var m = GetAll();
            return m.SingleOrDefault(expression.Compile());
        }
        
        public bool Any()
        {
            var m = GetAll();
            return m.Any();
        }
        
        public bool Any(Expression<Func<T, bool>> expression)
        {
            var m = GetAll();
            return m.Any(expression.Compile());
        }

        public async Task CreateAsync(T entity)
        {
            _messengerContext.GetSet<T>().Add(entity);
            await _messengerContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _messengerContext.GetSet<T>().Update(entity);
            await _messengerContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _messengerContext.GetSet<T>().Delete(entity);
            await _messengerContext.SaveChangesAsync();
        }
    }
}