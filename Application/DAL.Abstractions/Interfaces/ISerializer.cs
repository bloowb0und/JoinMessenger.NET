using System.Threading.Tasks;

namespace DAL.Abstractions.Interfaces
{
    public interface ISerializer
    {
        Task SaveToFileAsync<T>(T obj, string fileName);
        
        Task<T> LoadFromFileAsync<T>(string fileName) where T : new();
    }
}