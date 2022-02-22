using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using Core.Models;
using Core.Models.ServiceMethodsModels;
using DAL.Abstractions.Interfaces;
using FluentResults;

namespace BLL.Services
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChatService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> CreateChatAsync(Chat chat)
        {
            if (await _unitOfWork.ChatRepository.Any(c => c.Id == chat.Id))
            {
                return Result.Fail("Chat with this id already exists.");
            }

            if (await _unitOfWork.ChatRepository.Any(c => c.Server == chat.Server && c.Name == chat.Name))
            {
                return Result.Fail("Chat with this name already exists on the server.");
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

            return Result.Ok();
        }

        public async Task<Result> DeleteChatAsync(Chat chat)
        {
            if (!await _unitOfWork.ChatRepository.Any(c => c.Id == chat.Id))
            {
                return Result.Fail("This chat doesn't exist.");
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
            
            return Result.Ok();
        }

        public async Task<Result> EditChatAsync(Chat chat, ChatServiceEditChat newChat)
        {
            if (!await _unitOfWork.ChatRepository.Any(c => c.Id == chat.Id))
            {
                return Result.Fail("This chat doesn't exist.");
            }
            
            if (await _unitOfWork.ChatRepository.Any(c => c.Server == chat.Server && c.Name == newChat.ChatName))
            {
                return Result.Fail("Chat with this name already exists on the server.");
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
            
            return Result.Ok();
        }
    }
}