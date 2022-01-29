using BLL.Abstractions.Interfaces;
using Core.Models;
using DAL.Abstractions.Interfaces;

namespace BLL.Services
{
    public class ChatService : IChatService
    {
        private readonly IGenericRepository<Chat> _chatRepository;

        public ChatService(IGenericRepository<Chat> chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public bool CreateChat(Chat chat)
        {
            if (string.IsNullOrWhiteSpace(chat.Name)
                || chat.Server == null)
            {
                return false;
            }
            
            if (!_chatRepository.Any(c => c == chat))
            {
                return false;
            }

            _chatRepository.CreateAsync(chat);
            
            return true;
        }

        public bool DeleteChat(Chat chat)
        {
            if (string.IsNullOrWhiteSpace(chat.Name)
                || chat.Server == null)
            {
                return false;
            }
            
            if (!_chatRepository.Any(c => c == chat))
            {
                return false;
            }

            _chatRepository.DeleteAsync(chat);
            
            return true;
        }

        public bool EditChatName(Chat chat, string newChatName)
        {
            if (string.IsNullOrWhiteSpace(newChatName))
            {
                return false;
            }

            if (!_chatRepository.Any(c => c == chat))
            {
                return false;
            }
            
            chat.Name = newChatName;
            _chatRepository.UpdateAsync(chat);
            
            return true;
        }
    }
}