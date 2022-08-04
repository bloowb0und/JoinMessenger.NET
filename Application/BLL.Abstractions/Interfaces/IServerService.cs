using Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models.DTO;
using Core.Models.ServiceMethodsModels;
using FluentResults;

namespace BLL.Abstractions.Interfaces
{
    public interface IServerService
    {
        Task<Result<ServerDto>> CreateServerAsync(string name, User user);

        Task<Result> DeleteServerAsync(Server server, User user);

        Task<Result> AddUserAsync(Server server, User user);

        Task<Result> AddUsersAsync(Server server, IEnumerable<User> user);

        Task<Result> DeleteUserAsync(Server server, User user);

        Task<Result> DeleteUsersAsync(Server server, IEnumerable<User> user);

        Result<Server> GetServerById(int id);
        Result<Server> GetServerByName(string name);
        Task<Result<IEnumerable<ServerDto>>> GetServersForUser(User user);

        Task SendInvitationAsync(Server server, User user);

        Task<Result> EditServerAsync(Server server, ServerServiceEditServer newServer);
    }
}
