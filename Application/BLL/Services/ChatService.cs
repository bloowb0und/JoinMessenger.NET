using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using Core.Models;
using Core.Models.ServiceMethodsModels;
using DAL.Abstractions.Interfaces;

namespace BLL.Services
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChatService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CreateChatAsync(Chat chat)
        {
            if (await _unitOfWork.ChatRepository.Any(c => c.Id == chat.Id))
            {
                return false;
            }

            if (await _unitOfWork.ChatRepository.Any(c => c.Server == chat.Server && c.Name == chat.Name))
            {
                return false;
            }

            using (_unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    await _unitOfWork.ChatRepository.CreateAsync(chat);
                    await _unitOfWork.SaveAsync();

                    await _unitOfWork.CommitTransactionAsync();
                }
                catch 
                {
                    await _unitOfWork.RollbackTransactionAsync();
                }
            }
            
            return true;
        }

        public async Task<bool> DeleteChatAsync(Chat chat)
        {
            if (!await _unitOfWork.ChatRepository.Any(c => c == chat))
            {
                return false;
            }
            
            using (_unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    _unitOfWork.ChatRepository.Delete(chat);
                    await _unitOfWork.SaveAsync();

                    await _unitOfWork.CommitTransactionAsync();
                }
                catch 
                {
                    await _unitOfWork.RollbackTransactionAsync();
                }
            }
            
            return true;
        }

        public async Task<bool> EditChatAsync(Chat chat, ChatServiceEditChat newChat)
        {
            if (await _unitOfWork.ChatRepository.Any(c => c == chat))
            {
                return false;
            }
            
            if (await _unitOfWork.ChatRepository.Any(c => c.Server == chat.Server && c.Name == newChat.ChatName))
            {
                return false;
            }
            
            chat.Name = newChat.ChatName;
            
            using (_unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    _unitOfWork.ChatRepository.Update(chat);
                    await _unitOfWork.SaveAsync();

                    await _unitOfWork.CommitTransactionAsync();
                }
                catch 
                {
                    await _unitOfWork.RollbackTransactionAsync();
                }
            }
            
            return true;
        }
    }
}