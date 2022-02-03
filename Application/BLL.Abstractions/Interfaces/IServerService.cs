using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Abstractions.Interfaces
{
    public interface IServerService
    {
        Task<bool> CreateServer(string name);

        Task<bool> DeleteServer(Server server);

        Task<bool> AddUser(Server server, User user);

        Task<bool> AddUsers(Server server, IEnumerable<User> user);

        Task<bool> DeleteUser(Server server, User user);

        Task<bool> DeleteUsers(Server server, IEnumerable<User> user);

        Task SendInvitation(Server server, User user);
    }
}
