using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core;
using Core.Models;
using DAL.Abstractions.Interfaces;
using DAL.Contexts;
using Microsoft.Extensions.Options;

namespace DAL.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly AppSettings _appSettings;
        private readonly MessengerContext.DbSet<T> _dbSet;
        
        public GenericRepository(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));
            _dbSet = new MessengerContext.DbSet<T>(_appSettings.TempDirectory);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await Task.Run(() => _dbSet);
        }

        public async Task<IEnumerable<T>> FindByConditionAsync(Expression<Func<T, bool>> expression)
        {
            var m = await GetAllAsync();
            return m.Where(expression.Compile());
        }

        public async Task CreateAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            await DeleteAsync(entity);
            await CreateAsync(entity);
        }

        public async Task DeleteAsync(T entity)
        {
            await _dbSet.DeleteAsync(entity);
        }
    }
}