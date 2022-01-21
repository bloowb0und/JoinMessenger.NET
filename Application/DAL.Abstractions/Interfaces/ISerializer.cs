using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Abstractions.Interfaces
{
    public interface ISerializer
    {
        Task SaveToFileAsync<T>(T obj, string fileName);
        
        Task<IEnumerable<T>> LoadFromFileAsync<T>(string fileName);
    }
}