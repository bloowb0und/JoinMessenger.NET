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
        public bool CreateServer(string name);

        public bool DeleteServer(Server server);

        public bool AddUser(Server server, User user);

        public bool AddUsers(Server server, IEnumerable<User> user);

        public bool DeleteUser(Server server, User user);

        public bool DeleteUsers(Server server, IEnumerable<User> user);

        public Task SendInvitation(Server server, User user);
    }
}
