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
        private readonly IGenericRepository<Message> _messageRepository;

        public MessageService(IGenericRepository<Message> messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task<bool> CreateMessageAsync(Message message)
        {
            if (_messageRepository.Any(m => m == message))
            {
                return false;
            }

            await _messageRepository.CreateAsync(message);
            
            return true;
        }

        public async Task<bool> EditMessageAsync(User user, Message message, MessageServiceEditMessage newMessage)
        {
            if (!_messageRepository.Any(m => m == message))
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
            await _messageRepository.UpdateAsync(message);
            
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

            if (_messageRepository.Any(m => m == message))
            {
                return false;
            }

            // check if user sent this message [or has permission]
            if (message.User != user)
            {
                return false;
            }

            await _messageRepository.DeleteAsync(message);
            
            return true;
        }
    }
}