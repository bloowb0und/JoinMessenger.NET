using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using Core.Models;
using Core.Models.ServiceMethodsModels;
using DAL.Abstractions.Interfaces;

namespace BLL.Services
{
    public class MessageService : IMessageService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MessageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CreateMessageAsync(Message message)
        {
            if (await _unitOfWork.MessageRepository.Any(m => m.Id == message.Id))
            {
                return false;
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

            return true;
        }

        public async Task<bool> EditMessageAsync(User user, Message message, MessageServiceEditMessage newMessage)
        {
            if (!await _unitOfWork.MessageRepository.Any(m => m.Id == message.Id))
            {
                return false;
            }
            
            // check if user sent this message
            if (message.User != user)
            {
                return false;
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
            
            return true;
        }

        public async Task<bool> DeleteMessageAsync(User user, Message message)
        {
            if (message.User == null
                || message.Chat == null
                || string.IsNullOrWhiteSpace(message.Value))
            {
                return false;
            }

            if (await _unitOfWork.MessageRepository.Any(m => m.Id == message.Id))
            {
                return false;
            }

            // check if user sent this message [or has permission]
            if (message.User != user)
            {
                return false;
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
            
            return true;
        }
    }
}