using System.Threading.Tasks;
using Core.Models;

namespace BLL.Abstractions.Interfaces
{
    public interface IMessageService
    {
        Task<bool> CreateMessage(Message message);
        Message? GetMessageById(int id);
        Task<bool> EditMessage(Message message, string newValue);
        Task<bool> DeleteMessage(User user, Message message);
    }
}