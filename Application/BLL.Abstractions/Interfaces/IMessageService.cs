using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Core.Models;
using Core.Models.ServiceMethodsModels;

namespace BLL.Abstractions.Interfaces
{
    public interface IMessageService
    {
        Task<bool> CreateMessageAsync(Message message);
        Message GetMessageById(int id);
        Task<bool> EditMessageAsync(User user, Message message, MessageServiceEditMessage newMessage);
        Task<bool> DeleteMessageAsync(User user, Message message);
    }
}