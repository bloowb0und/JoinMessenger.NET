using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;

namespace DAL.Contexts
{
    public class MessengerContext
    {

        public class DbSet<T> : IEnumerable<T> where T : BaseEntity
        {
            private readonly string _pathToFile;
            
            private readonly MyJsonSerializer _jsonSerializer;
            
            public DbSet(string pathToFile)
            {
                _pathToFile = pathToFile;
                _jsonSerializer = new MyJsonSerializer();
            }

            private class CustomEnumerator : IEnumerator<T>, IEnumerator
            {
                private string _pathToFile;
                
                private IEnumerable<T> _list;
                
                private int _index;
                
                private T _current;

                public T Current => _current!;

                object? IEnumerator.Current
                {
                    get
                    {
                        if (_index == 0 || _index == _list.Count() + 1)
                        {
                            throw new InvalidOperationException("Enumerator reached end of IEnumerable.");
                        }
                        
                        return Current;
                    }
                }
                public CustomEnumerator(string pathToFile)
                {
                    _pathToFile = pathToFile;
                    _list = new MyJsonSerializer().LoadFromFileAsync<T>(_pathToFile).Result;
                    _index = 0;
                    _current = default;
                }

                public bool MoveNext()
                {
                    IEnumerable<T> localList = _list.ToList();

                    if (_index < localList.Count())
                    {
                        _current = localList.ElementAt(_index);
                        _index++;
                        return true;
                    }

                    _index = localList.Count() + 1;;
                    _current = default;
                    return false;
                }

                public void Reset()
                {
                    _index = 0;
                    _current = default;
                }

                public void Dispose()
                {
                }
            }

            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                return new CustomEnumerator(_pathToFile);
            }

            public IEnumerator GetEnumerator()
            {
                return new CustomEnumerator(_pathToFile);
            }

            public async Task AddAsync(T entity)
            {
                if (this.Any(obj => obj.Id == entity.Id))
                {
                    throw new ArgumentException($"Element with same id as {entity} exists.");
                }
                
                await _jsonSerializer.SaveToFileAsync(entity, _pathToFile);
            }

            public async Task DeleteAsync(T entity)
            {
                if (!this.Contains(entity))
                {
                    throw new ArgumentException($"Object {entity} doesn't exists.");
                }
                
                await _jsonSerializer.DeleteFromFileAsync(entity, _pathToFile);
            }
        }
    }
}