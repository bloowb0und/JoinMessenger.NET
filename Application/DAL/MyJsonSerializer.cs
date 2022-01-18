using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using DAL.Abstractions.Interfaces;
using Newtonsoft.Json;

namespace DAL
{
    public class MyJsonSerializer : ISerializer
    {

        public MyJsonSerializer()
        {
        }

        public async Task SaveToFileAsync<T>(T obj, string fileName)
        {
            var json = await File.ReadAllTextAsync(fileName);
            var dictionary = JsonConvert.DeserializeObject<Dictionary<Type, IList>>(json);
            var genericType = typeof(T).GetGenericArguments()[0];

            if (!dictionary.TryAdd(genericType, (IList)obj))
            {
                dictionary.Remove(genericType);
                dictionary.Add(genericType, (IList)obj);
            }
            
            json = JsonConvert.SerializeObject(dictionary, Formatting.Indented);
            
            await File.WriteAllTextAsync(fileName,json);
        }

        public async Task<T> LoadFromFileAsync<T>(string fileName) where T : new()
        {
            var json = await File.ReadAllTextAsync(fileName);
            
            if (string.IsNullOrEmpty(json) || string.IsNullOrWhiteSpace(json))
            {
                json = JsonConvert.SerializeObject(new Dictionary<Type, IList<BaseEntity>>());
            }
            
            var result = JsonConvert.DeserializeObject<Dictionary<Type, T>>(json) 
                         ?? new Dictionary<Type, T>();
            
            return result.ToList().FirstOrDefault(pair => pair.Key == typeof(T).GetGenericArguments()[0]).Value
                ?? new T(); 
        }
    }
}