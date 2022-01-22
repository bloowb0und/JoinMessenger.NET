using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Core.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace DAL.Contexts
{
    public class MessengerContext
    {
        private readonly AppSettings _appSettings;
        private JObject _jObject;
        private readonly Dictionary<Type, DbSet> _dbSetCollection = new Dictionary<Type, DbSet>();
        public MessengerContext(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));
        }

        public DbSet<T> GetSet<T>() where T : BaseEntity
        {
            this.Initialize().ConfigureAwait(false).GetAwaiter().GetResult();

            if (_dbSetCollection.ContainsKey(typeof(T)))
            {
                return (DbSet<T>) this._dbSetCollection[typeof(T)];
            }

            if (_jObject.ContainsKey(typeof(T).Name))
            {
                var dbSet = new DbSet<T>(this._jObject[typeof(T).Name]);
                _dbSetCollection.Add(typeof(T), dbSet);

                return dbSet;
            }
            else
            {
                var jArray = new JArray();
                _jObject.Add(typeof(T).Name, jArray);

                var dbSet = new DbSet<T>(jArray);
                _dbSetCollection.Add(typeof(T), dbSet);

                return dbSet;
            }
        }

        public async Task SaveChangesAsync()
        {
            foreach (var pair in _dbSetCollection)
            {
                await pair.Value.SaveChangesAsync();
            }

            if (File.Exists(_appSettings.TempDirectory))
            {
                await File.WriteAllTextAsync(_appSettings.TempDirectory, _jObject.ToString());
            }
        }

        private bool Initialized = false;
        private async Task Initialize()
        {
            if (Initialized)
            {
                return;
            }

            if (File.Exists(_appSettings.TempDirectory))
            {
                string json = await File.ReadAllTextAsync(_appSettings.TempDirectory);
                _jObject = JObject.Parse(json);
            }
            else
            {
                _jObject = new JObject();
                await File.WriteAllTextAsync(_appSettings.TempDirectory, _jObject.ToString());
            }
            
            Initialized = true;
        }

        public class DbSet
        {
            protected readonly JArray _root;

            public DbSet(JToken root)
            {
                _root = (JArray)root;
            }

            public virtual IEnumerable GetAll()
            {
                throw new NotSupportedException();
            }
            
            public virtual async Task SaveChangesAsync()
            {
                throw new NotSupportedException();
            }
        }

        public class DbSet<T> : DbSet, IEnumerable<T> where T : BaseEntity 
        {
            private readonly List<T> _collectionToAdd = new List<T>();
            private readonly List<T> _collectionToRemove = new List<T>();
            
            public DbSet(JToken root) : base (root)
            {
                
            }

            public void Update(T entity)
            {
                JToken token = _root.FirstOrDefault(t => t.ToObject<T>().Id == entity.Id);
                
                if (token == null)
                {
                    throw new ArgumentException($"Object {entity} doesn't exist");
                }

                _collectionToRemove.Add(token.ToObject<T>());
                _collectionToAdd.Add(entity);            
            }
            
            public void Delete(T entity)
            {
                _collectionToRemove.Add(entity);
            }
            
            public void Add(T entity)
            {
                this._collectionToAdd.Add(entity);
            }
            
            public override IEnumerable GetAll()
            {
                return _root.ToObject<IEnumerable<T>>();
            }
            
            public override async Task SaveChangesAsync()
            {
                foreach (var el in this._collectionToAdd)
                {
                    this._root.Add(JToken.FromObject(el));
                }
                
                this._collectionToAdd.Clear();
                
                foreach (var el in this._collectionToRemove)
                {
                    this._root.Remove(JToken.FromObject(el));
                }
                
                this._collectionToRemove.Clear();
            }
            
            private class CustomEnumerator : IEnumerator<T>
            {
                private IEnumerable<T> _localList;
                
                private int _index;
                
                private T _current;

                public T Current => _current!;

                object? IEnumerator.Current
                {
                    get
                    {
                        if (_index == 0 || _index == _localList.Count() + 1)
                        {
                            throw new InvalidOperationException("Enumerator reached end of IEnumerable.");
                        }
                        
                        return Current;
                    }
                }

                public CustomEnumerator(DbSet<T> dbSet)
                {
                    _localList = dbSet;
                    _index = 0;
                    _current = default;
                }

                public bool MoveNext()
                {
                    IEnumerable<T> localList = _localList.ToList();

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

            public IEnumerator<T> GetEnumerator()
            {
                return new CustomEnumerator(this);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}