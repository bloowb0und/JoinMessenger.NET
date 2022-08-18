using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;
using Core.Models.ServiceMethodsModels;
using FluentResults;

namespace BLL.Abstractions.Interfaces
{
    public interface IChatService
    {
        Task<Result<Chat>> CreateChatAsync(string name, ChatType type, Server server);
        Task<Result> DeleteChatAsync(Chat chat);
        Task<Result> EditChatAsync(Chat chat, ChatServiceEditChat newChat);
        Task<Result<IEnumerable<Chat>>> GetChatsByServerAsync(int serverId);
        Result<Chat> GetChatById(int chatId);
    }
}