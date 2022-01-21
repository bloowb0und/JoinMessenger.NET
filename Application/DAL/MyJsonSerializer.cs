using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using DAL.Abstractions.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DAL
{
    public class MyJsonSerializer : ISerializer
    {

        public MyJsonSerializer()
        {
        }

        public async Task DeleteFromFileAsync<T>(T obj, string fileName)
        {
            if (!File.Exists(fileName))
            {
                await File.WriteAllTextAsync(fileName, 
                    JsonConvert.SerializeObject(new Dictionary<string, IList<BaseEntity>>()));
                throw new InvalidDataException($"Object {obj.ToString()} doesn't exist.");
            }
            var json = await File.ReadAllTextAsync(fileName);
            JObject jObject = JObject.Parse(json);

            if (!jObject.ContainsKey(typeof(T).ToString()) 
                || !jObject[typeof(T).ToString()].Children().Contains(JObject.FromObject(obj)))
            {
                throw new InvalidDataException($"Object {obj.ToString()} doesn't exist.");
            }

            jObject[typeof(T).ToString()] = jObject[typeof(T).ToString()].Children()
                .Where(obj => obj.Equals(JObject.FromObject(obj))) as JToken;
            json = jObject.ToString();
            
            await File.WriteAllTextAsync(fileName,json);
        }
        
        public async Task SaveToFileAsync<T>(T obj, string fileName)
        {
            if (!File.Exists(fileName))
            {
                await File.WriteAllTextAsync(fileName, 
                    JsonConvert.SerializeObject(new Dictionary<string, IList<BaseEntity>>()));
            }
            
            var json = await File.ReadAllTextAsync(fileName);
            JObject jObject = JObject.Parse(json);

            if (jObject.ContainsKey(typeof(T).ToString()))
            {
                JToken ie = jObject[typeof(T).ToString()] ?? JToken.FromObject(new List<T>());
                JToken newObj = JToken.FromObject(obj);
                jObject[typeof(T).ToString()].Replace(JToken.FromObject(ie.Append(newObj)));
            }
            else
            {
                List<T> list = new List<T>();
                list.Add(obj);
                jObject.Add(typeof(T).ToString(), JToken.FromObject(list));
            }
            
            json = jObject.ToString();
            
            await File.WriteAllTextAsync(fileName,json);
        }

        public async Task<IEnumerable<T>> LoadFromFileAsync<T>(string fileName)
        {
            if (!File.Exists(fileName))
            {
                await File.WriteAllTextAsync(fileName, 
                    JsonConvert.SerializeObject(new Dictionary<string, IList<BaseEntity>>()));
                return new List<T>();
            }
            
            var json = await File.ReadAllTextAsync(fileName);
            
            if (string.IsNullOrEmpty(json) || string.IsNullOrWhiteSpace(json))
            {
                json = JsonConvert.SerializeObject(new Dictionary<string, IList<BaseEntity>>());
            }
            
            JObject jObject = JObject.Parse(json);

            IList<JToken> results;
            if(jObject.ContainsKey(typeof(T).ToString()))
            {
                results = jObject[typeof(T).ToString()].Children().ToList();
            }
            else
            {
                return new List<T>();
            }

            IList<T> searchResults = new List<T>();
            foreach (JToken r in results)
            {
                T searchResult = r.ToObject<T>();
                searchResults.Add(searchResult);
            }

            return searchResults;
        }
    }
}