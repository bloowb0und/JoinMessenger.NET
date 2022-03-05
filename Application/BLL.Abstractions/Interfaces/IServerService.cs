using Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models.ServiceMethodsModels;
using FluentResults;

namespace BLL.Abstractions.Interfaces
{
    public interface IServerService
    {
        Task<Result> CreateServerAsync(string name);

        Task<Result> DeleteServerAsync(Server server);

        Task<Result> AddUserAsync(Server server, User user);

        Task<Result> AddUsersAsync(Server server, IEnumerable<User> user);

        Task<Result> DeleteUserAsync(Server server, User user);

        Task<Result> DeleteUsersAsync(Server server, IEnumerable<User> user);

        Result<Server> GetServerByName(string name);

        Task SendInvitationAsync(Server server, User user);

        Task<Result> EditServerAsync(Server server, ServerServiceEditServer newServer);
    }
}
