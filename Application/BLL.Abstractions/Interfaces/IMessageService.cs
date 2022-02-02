using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Core.Models;

namespace BLL.Abstractions.Interfaces
{
    public interface IMessageService
    {
        Task<bool> CreateMessageAsync(Message message);
        Message GetMessageById(int id);
        Task<bool> EditMessageAsync(User user, Message message, string newValue);
        Task<bool> DeleteMessageAsync(User user, Message message);
    }
}