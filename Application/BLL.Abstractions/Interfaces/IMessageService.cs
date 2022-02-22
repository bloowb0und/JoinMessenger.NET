using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Core.Models;
using Core.Models.ServiceMethodsModels;
using FluentResults;

namespace BLL.Abstractions.Interfaces
{
    public interface IMessageService
    {
        Task<Result> CreateMessageAsync(Message message);
        Task<Result> EditMessageAsync(User user, Message message, MessageServiceEditMessage newMessage);
        Task<Result> DeleteMessageAsync(User user, Message message);
    }
}