using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core;
using Core.Models;
using DAL.Abstractions.Interfaces;
using Microsoft.Extensions.Options;

namespace DAL.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly AppSettings _appSettings;
        private readonly ISerializer _serializer;
        private List<T> _allData;
        
        public GenericRepository(ISerializer serializer, IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));
            _serializer = serializer;
            _allData = new List<T>();
        }

        public async Task<IEnumerable<T>> FindAllAsync()
        {
            return await _serializer.LoadFromFileAsync<List<T>>(_appSettings.TempDirectory);
        }

        public async Task<IEnumerable<T>> FindByConditionAsync(Expression<Func<T, bool>> expression)
        {
            var m = await FindAllAsync();
            return m.Where(expression.Compile());
        }

        public async Task CreateAsync(T entity)
        {
            _allData = (await FindAllAsync()).ToList();
            _allData.Add(entity);
            await _serializer.SaveToFileAsync(_allData, _appSettings.TempDirectory);
        }

        public async Task UpdateAsync(T entity)
        {
            await DeleteAsync(entity);
            await CreateAsync(entity);
        }

        public async Task DeleteAsync(T entity)
        {
            _allData = (await FindAllAsync()).ToList();
            
            var k = _allData.FirstOrDefault(o => o.Id == entity.Id);
            _allData.Remove(k);
            
            await _serializer.SaveToFileAsync(_allData, _appSettings.TempDirectory);
        }
    }
}