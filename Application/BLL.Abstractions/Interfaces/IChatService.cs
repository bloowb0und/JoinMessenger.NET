using System.Threading.Tasks;
using Core.Models;
using Core.Models.ServiceMethodsModels;
using FluentResults;

namespace BLL.Abstractions.Interfaces
{
    public interface IChatService
    {
        Task<Result> CreateChatAsync(Chat chat);
        Task<Result> DeleteChatAsync(Chat chat);
        Task<Result> EditChatAsync(Chat chat, ChatServiceEditChat newChat);
    }
}