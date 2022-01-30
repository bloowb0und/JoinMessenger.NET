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
        private readonly string _connectionString;

        private JObject _jObject;
        
        private readonly Dictionary<Type, DbSet> _dbSetCollection = new Dictionary<Type, DbSet>();
        
        public MessengerContext(string appSettings)
        {
            _connectionString = appSettings;
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

            if (File.Exists(_connectionString))
            {
                await File.WriteAllTextAsync(_connectionString, _jObject.ToString());
            }
        }

        private bool Initialized = false;
        
        private async Task Initialize()
        {
            if (Initialized)
            {
                return;
            }

            if (File.Exists(_connectionString))
            {
                string json = await File.ReadAllTextAsync(_connectionString);
                _jObject = JObject.Parse(json);
            }
            else
            {
                _jObject = new JObject();
                await File.WriteAllTextAsync(_connectionString, _jObject.ToString());
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

            public virtual async Task SaveChangesAsync()
            {
                throw new NotSupportedException();
            }
        }

        public class DbSet<T> : DbSet, IEnumerable<T> where T : BaseEntity 
        {
            private readonly List<T> _collectionToAdd = new List<T>();
            
            private readonly List<JToken> _collectionToRemove = new List<JToken>();
            
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

                _collectionToRemove.Add(token);
                _collectionToAdd.Add(entity);            
            }
            
            public void Delete(T entity)
            {
                JToken token = _root.FirstOrDefault(t => t.ToObject<T>().Id == entity.Id);
                
                if (token == null)
                {
                    throw new ArgumentException($"Object {entity} doesn't exist");
                }

                _collectionToRemove.Add(token);
            }
            
            public void Add(T entity)
            {
                // var lastId = _root.LastOrDefault().ToObject<T>().Id;
                if (_root.LastOrDefault() != null)
                {
                    entity.Id =_root.LastOrDefault().ToObject<T>().Id + 1;
                }

                if (_root.Any(t => t.ToObject<T>().Id == entity.Id))
                {
                    throw new ArgumentException($"Object with same id as {entity} already exists");
                }
                
                this._collectionToAdd.Add(entity);
            }

            public override async Task SaveChangesAsync()
            {
                foreach (var entity in this._collectionToAdd)
                {
                    this._root.Add(JToken.FromObject(entity));
                }
                
                this._collectionToAdd.Clear();
                
                foreach (var token in this._collectionToRemove)
                {
                    this._root.Remove(token);
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
                    _localList = dbSet._root.ToObject<IEnumerable<T>>();
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