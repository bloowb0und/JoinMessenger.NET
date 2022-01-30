using Core.Models;

namespace BLL.Abstractions.Interfaces
{
    public interface IChatService
    {
        bool CreateChat(Chat chat);
        Chat? GetChatById(int id);
        bool DeleteChat(Chat chat);
        bool EditChatName(Chat chat, string newChatName);
    }
}