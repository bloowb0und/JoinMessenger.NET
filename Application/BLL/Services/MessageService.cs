using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using Core.Models;
using Core.Models.ServiceMethodsModels;
using DAL.Abstractions.Interfaces;
using FluentResults;

namespace BLL.Services
{
    public class MessageService : IMessageService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MessageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Message>> CreateMessageAsync(int userId, int chatId, string value)
        {
            var user = _unitOfWork.UserRepository.FirstOrDefault(u => u.Id == userId);
            var chat = _unitOfWork.ChatRepository.FirstOrDefault(c => c.Id == chatId);

            if (user == null
                || chat == null)
            {
                return Result.Fail("User or chat was not found.");
            }

            var message = new Message
            {
                User = user,
                Chat = chat,
                DateCreated = DateTime.Now,
                Value = value
            };
            
            using (_unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    await _unitOfWork.MessageRepository.CreateAsync(message);
                    await _unitOfWork.SaveAsync();

                    await _unitOfWork.CommitTransactionAsync();
                }
                catch 
                {
                    await _unitOfWork.RollbackTransactionAsync();
                }
            }

            return Result.Ok(message);
        }

        public async Task<Result> EditMessageAsync(User user, int messageId, EditMessageModel newEditMessage)
        {
            Message foundMessage;
            
            if ((foundMessage = _unitOfWork.MessageRepository.FirstOrDefault(m => m.Id == messageId)) == null)
            {
                return Result.Fail("Message with such id doesn't exist.");
            }
            
            // check if user sent this message
            if (foundMessage.User != user)
            {
                return Result.Fail("Can't edit another user's message.");
            }

            foundMessage.Value = newEditMessage.MessageValue;
            foundMessage.DateLastEdited = DateTime.Now;
            
            using (_unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    _unitOfWork.MessageRepository.Update(foundMessage);
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

        public async Task<Result> DeleteMessageAsync(User user, int messageId)
        {
            Message foundMessage;
            
            if ((foundMessage = _unitOfWork.MessageRepository.FirstOrDefault(m => m.Id == messageId)) == null)
            {
                return Result.Fail("Message with such id doesn't exist.");
            }

            // check if user sent this message [or has permission]
            if (foundMessage.User != user)
            {
                return Result.Fail("No permission to delete this message.");
            }
            
            using (_unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    _unitOfWork.MessageRepository.Delete(foundMessage);
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

        public Result<Message> GetMessageById(int messageId)
        {
            var message = _unitOfWork.MessageRepository.FirstOrDefault(m => m.Id == messageId);

            if (message == null)
            {
                return Result.Fail("Message with such id doesn't exist.");
            }

            return Result.Ok(message);
        }
        
        public async Task<Result<IEnumerable<Message>>> GetAllMessagesFromChatAsync(int chatId)
        {
            var chat = _unitOfWork.ChatRepository.FirstOrDefault(c => c.Id == chatId);

            if (chat == null)
            {
                return Result.Fail("Chat with such id doesn't exist.");
            }

            var messages = await _unitOfWork.MessageRepository.Get(m => m.Chat.Id == chatId);

            return Result.Ok(messages);
        }
    }
}