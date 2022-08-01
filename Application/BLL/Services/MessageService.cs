using System;
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

        public async Task<Result> CreateMessageAsync(Message message)
        {
            if (await _unitOfWork.MessageRepository.Any(m => m.Id == message.Id))
            {
                return Result.Fail("Message with such id already exists.");
            }
            
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

            return Result.Ok();
        }

        public async Task<Result> EditMessageAsync(User user, Message message, MessageServiceEditMessage newMessage)
        {
            if (!await _unitOfWork.MessageRepository.Any(m => m.Id == message.Id))
            {
                return Result.Fail("Such message doesn't exist.");
            }
            
            // check if user sent this message
            if (message.User != user)
            {
                return Result.Fail("Can't edit another user's message.");
            }

            message.Value = newMessage.MessageValue;
            message.DateLastEdited = DateTime.Now;
            
            using (_unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    _unitOfWork.MessageRepository.Update(message);
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

        public async Task<Result> DeleteMessageAsync(User user, Message message)
        {
            if (message.User == null
                || message.Chat == null
                || string.IsNullOrWhiteSpace(message.Value))
            {
                return Result.Fail("Input data is null or the message is empty.");
            }

            if (!await _unitOfWork.MessageRepository.Any(m => m.Id == message.Id))
            {
                return Result.Fail("Message doesn't exist.");
            }

            // check if user sent this message [or has permission]
            if (message.User != user)
            {
                return Result.Fail("No permission to delete this message.");
            }
            
            using (_unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    _unitOfWork.MessageRepository.Delete(message);
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