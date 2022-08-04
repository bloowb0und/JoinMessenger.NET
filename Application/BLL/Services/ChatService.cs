using System.Collections.Generic;
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

        public async Task<Result<Chat>> CreateChatAsync(string name, ChatType type, Server server)
        {
            if (await _unitOfWork.ChatRepository.Any(c => c.Server == server && c.Name == name))
            {
                return Result.Fail("Chat with this name already exists on the server.");
            }

            using (_unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    await _unitOfWork.ChatRepository.CreateAsync(new Chat()
                    {
                        Name = name,
                        Type = type,
                        Server = server
                    });
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

        public async Task<Result<IEnumerable<Chat>>> GetChatsByServerAsync(int serverId)
        {
            var foundServer = _unitOfWork.ServerRepository.FirstOrDefault(s => s.Id == serverId);
            
            if (foundServer == null)
            {
                return Result.Fail("Server with such id doesn't exist.");
            }
            
            var chats = await _unitOfWork.ChatRepository.Get(c => c.Server.Id == serverId);
            
            return Result.Ok(chats);
        }

        public Result<Chat> GetChatById(int chatId)
        {
            var foundChat = _unitOfWork.ChatRepository.FirstOrDefault(c => c.Id == chatId);

            if (foundChat == null)
            {
                return Result.Fail("Chat with such id wasn't found");
            }
            
            return Result.Ok(foundChat);
        }
    }
}