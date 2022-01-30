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
        bool CreateServer(string name);

        bool DeleteServer(Server server);

        bool AddUser(Server server, User user);

        bool AddUsers(Server server, IEnumerable<User> user);

        bool DeleteUser(Server server, User user);

        bool DeleteUsers(Server server, IEnumerable<User> user);

        Task SendInvitation(Server server, User user);
    }
}
