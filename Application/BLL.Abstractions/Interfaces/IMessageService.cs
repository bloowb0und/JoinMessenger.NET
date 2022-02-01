using System.Threading.Tasks;
using Core.Models;

namespace BLL.Abstractions.Interfaces
{
    public interface IMessageService
    {
        Task<bool> CreateMessage(Message message);
        Message? GetMessageById(int id);
        Task<bool> EditMessage(User user, Message message, string newValue);
        Task<bool> DeleteMessage(User user, Message message);
    }
}