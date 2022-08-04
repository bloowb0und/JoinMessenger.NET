using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;
using Core.Models.ServiceMethodsModels;
using FluentResults;

namespace BLL.Abstractions.Interfaces
{
    public interface IMessageService
    {
        Task<Result<Message>> CreateMessageAsync(int userId, int chatId, string value);
        Task<Result> EditMessageAsync(User user, int messageId, EditMessageModel newEditMessage);
        Task<Result> DeleteMessageAsync(User user, int messageId);
        Result<Message> GetMessageById(int messageId);
        Task<Result<IEnumerable<Message>>> GetAllMessagesFromChatAsync(int chatId);
    }
}