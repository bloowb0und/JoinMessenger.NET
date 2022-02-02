using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Core.Models;

namespace BLL.Abstractions.Interfaces
{
    public interface IChatService
    {
        Task<bool> CreateChatAsync(Chat chat);
        Chat GetChatById(int id);
        Task<bool> DeleteChatAsync(Chat chat);
        Task<bool> EditChatNameAsync(Chat chat, string newChatName);
    }
}