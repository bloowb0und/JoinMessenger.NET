using Core.Models;

namespace BLL.Abstractions.Interfaces
{
    public interface IChatService
    {
        bool CreateChat(Chat chat);
        bool DeleteChat(Chat chat);
        bool EditChatName(Chat chat, string newChatName);
    }
}