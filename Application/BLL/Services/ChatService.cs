using System.Linq;
using System.Threading.Tasks;
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

        public async Task<bool> CreateChatAsync(Chat chat)
        {
            if (_chatRepository.Any(c => c == chat))
            {
                return false;
            }

            if (_chatRepository.Any(c => c.Server == chat.Server && c.Name == chat.Name))
            {
                return false;
            }

            await _chatRepository.CreateAsync(chat);
            
            return true;
        }

        public Chat GetChatById(int id)
        {
            if (!_chatRepository.Any(c => c.Id == id))
            {
                return null;
            }

            return _chatRepository.FindByCondition(c => c.Id == id).First();
        }

        public async Task<bool> DeleteChatAsync(Chat chat)
        {
            if (!_chatRepository.Any(c => c == chat))
            {
                return false;
            }

            await _chatRepository.DeleteAsync(chat);
            
            return true;
        }

        public async Task<bool> EditChatNameAsync(Chat chat, string newChatName)
        {
            if (_chatRepository.Any(c => c == chat))
            {
                return false;
            }
            
            if (_chatRepository.Any(c => c.Server == chat.Server && c.Name == newChatName))
            {
                return false;
            }
            
            chat.Name = newChatName;
            await _chatRepository.UpdateAsync(chat);
            
            return true;
        }
    }
}